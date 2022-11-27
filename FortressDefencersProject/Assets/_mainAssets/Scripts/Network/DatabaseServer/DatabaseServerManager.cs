using DevourDev.MonoBase;
using DevourDev.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using FD.Networking.Database.Packets;
using DevourDev.MonoExtentions;

namespace FD.Networking.Database
{
    //public class GameMatchesDatabase : DevourDev.Database.Database<Match>
    public class DatabaseServerManager : MonoBehaviour
    {
        public event Action<IPacketContent, ResponsingConnection> OnRequestReceived;
        public event Action<Entities.Account.Account> OnNewAccountRegistrated;

        [SerializeField] private int _port = 11_111;
        [SerializeField] private int _maxConnectionsQ = 5;
        [SerializeField] private int _adminConnectionBufferSize = 8096;
        [SerializeField] private string[] _allowedIpStrings = { "127.0.0.1", "178.140.136.189", "85.30.248.243" };

        private IPAddress[] _allowedIps;
        private ConnectionsHandlerBase _adminConnectionsHandler;
        private Packets.FD_DatabasePacketsResolver _packetsResolver;

        private AccountsDatabase _database; //TODO: add path to check backups
        private List<ResponsingConnection> _adminConnections;


        public AccountsDatabase Database => _database;

        //private void OnApplicationQuit()
        //{
        //    if (_adminConnectionsHandler != null && _adminConnectionsHandler.MainSocket != null)
        //    {
        //        DevNet.CloseSocket(_adminConnectionsHandler.MainSocket);
        //    }
        //}

        private void OnApplicationQuit()
        {
            _adminConnectionsHandler.Dispose();
            foreach (var ac in _adminConnections)
            {
                ac.Dispose();
            }

            _database.SaveEntities(@"OnExitBackups\", "b_" + DateTime.Now.ToLongDateString());
        }

        private void Awake()
        {
            _packetsResolver = new();
            _database = new();
            ParseAllowedIps();
            _adminConnectionsHandler = new(_port, _maxConnectionsQ);
            _adminConnections = new();
        }

        private void Start()
        {
            _adminConnectionsHandler.OnNewConnection += HandleNewAdminConnection;
            _adminConnectionsHandler.StartAccepting();
        }

        private void HandleNewAdminConnection(Socket handler)
        {
            var admCon = new ResponsingConnection(handler, _adminConnectionBufferSize, _packetsResolver);
            _adminConnections.Add(admCon);
            admCon.OnError += HandleAdminConnectionError;
            admCon.OnRequestReceived += HandleAdminRequest;
            admCon.StartHandling();

        }

        private void HandleAdminRequest(IPacketContent content, ResponsingConnection connection)
        {
            this.InvokeOnMainThread(() => OnRequestReceived?.Invoke(content, connection));
            switch (content)
            {
                case Packets.GetAccountDataRequest getAccDataRequest:
                    HandleGetAccountDataRequest(getAccDataRequest, connection);
                    break;
                case Packets.SetAccountDataRequest setAccDataRequest:
                    HandleSetAccountDataRequest(setAccDataRequest, connection);
                    break;
                case Packets.HandleLogInRequest handleLogInRequest:
                    Handle_HandleLogInRequest(handleLogInRequest, connection);
                    break;
                case Packets.HandleSignUpRequest handleSignUpRequest:
                    Handle_HandleSignUpRequest(handleSignUpRequest, connection);
                    break;

                case RegistrateGameOverRequest registrateGameOverRequest:
                    HandleRegistrateGameOverRequest(registrateGameOverRequest, connection);
                    break;
                default:
                    break;
            }
        }

        private void HandleRegistrateGameOverRequest(RegistrateGameOverRequest req, ResponsingConnection connection)
        {
            var response = new RegistrateGameOverResponse();

            if (!_database.TryFindByID(req.AccID, out var acc))
            {
                this.InvokeOnMainThread(() => Debug.Log($"unable to find acc with id {req.AccID}"));
                response.Result = false;
                connection.Connection.Response(response);
                return;
            }

            acc.GameStatistics.Mmr += req.MmrChange;
            acc.GameStatistics.GamesHistory.Add(req.MatchID);
            acc.GameStatistics.Total += 1;

            if (req.MatchResult == MatchResult.Win)
                acc.GameStatistics.Wins += 1;

            response.Result = true;
            connection.Connection.Response(response);
        }

        private void HandleSetAccountDataRequest(SetAccountDataRequest request, ResponsingConnection connection)
        {
            var response = new SetAccountDataResponse();
            Entities.Account.Account acc;
            switch (request.SearchMode)
            {
                case AccountSearchMode.ByID:
                    _database.TryFindByID(request.AccountID, out acc);
                    break;
                case AccountSearchMode.ByLogin:
                    _database.TryFindByLogin(request.AccountLogin, out acc);
                    break;
                default:
                    Debug.LogError("Unexpected enum value: " + request.ToString());
                    throw new Exception();
            }

            if (acc == null)
            {
                response.Result = false;
                response.FailReason = SetAccountDataResponse.Error.AccountNotFound;
                connection.Connection.Response(response);
                return;
            }

            var s = request.Settings;

            if (s.SetPublicInfo)
            {
                acc.PublicData = s.Value.PublicData;
            }
            if (s.SetLogInData)
            {
                acc.SecureData.LogInData = s.Value.SecureData.LogInData;
            }
            if (s.SetSecureHistory)
            {
                switch (s.Mode)
                {
                    case DevourDev.Database.Interfaces.SetEntityMode.Add:
                        acc.SecureData.History.ActionsHistory.AddRange(s.Value.SecureData.History.ActionsHistory);
                        break;
                    case DevourDev.Database.Interfaces.SetEntityMode.Replace:
                        acc.SecureData.History = s.Value.SecureData.History;
                        break;
                    default:
                        break;
                }
            }
            if (s.SetTemporary)
            {
                if (acc.Temporary == null)
                {
                    acc.Temporary = s.Value.Temporary;
                }
                else
                {
                    switch (s.Mode)
                    {
                        case DevourDev.Database.Interfaces.SetEntityMode.Add:
                            foreach (var k in s.Value.Temporary.Keys)
                            {
                                acc.Temporary.SetKey(k);
                            }
                            break;
                        case DevourDev.Database.Interfaces.SetEntityMode.Replace:
                            break;
                        default:
                            break;
                    }
                }
            }

            response.Result = true;
            connection.Connection.Response(response);
        }

        private void Handle_HandleSignUpRequest(HandleSignUpRequest request, ResponsingConnection connection)
        {
            var response = new HandleSignUpResponse();

            if (!_database.TryRegistrateAccount(request.Login, request.ReHashedPassword, request.Email, out var acc))
            {
                response.Result = false;
                response.FailReason = HandleSignUpResponse.Error.UnavailableLogin;
                goto Responsing;
            }

            acc.GameStatistics.Mmr = 1000 + new System.Random().Next(-500, 501);

            response.Result = true;
            response.SessionKey = _database.UpdateEKey(acc, Entities.Account.EKeyType.GardenSession).Key;
            this.InvokeOnMainThread(() => OnNewAccountRegistrated?.Invoke(acc));
        Responsing:

            connection.Connection.Response(response);

        }

        private void Handle_HandleLogInRequest(HandleLogInRequest request, ResponsingConnection connection)
        {
            var response = new HandleLogInResponse();

            if (!_database.TryFindByLogin(request.Login, out var acc))
            {
                response.Result = false;
                response.FailReason = HandleLogInResponse.Error.AccountNotFound;
                goto Responsing;
            }

            if (request.ReHashedPassword != acc.SecureData.LogInData.HashedPassword)
            {
                response.Result = false;
                response.FailReason = HandleLogInResponse.Error.WrongPassword;
                goto Responsing;
            }

            response.Result = true;
            response.SessionKey = _database.UpdateEKey(acc, Entities.Account.EKeyType.GardenSession).Key;
            goto Responsing;


        Responsing:

            connection.Connection.Response(response);
        }

        private void HandleGetAccountDataRequest(GetAccountDataRequest request, ResponsingConnection connection)
        {
            var response = new GetAccountDataResponse();
            Entities.Account.Account acc;
            switch (request.SearchMode)
            {
                case AccountSearchMode.ByID:
                    _database.TryFindByID(request.AccountID, out acc);
                    break;
                case AccountSearchMode.ByLogin:
                    _database.TryFindByLogin(request.AccountLogin, out acc);
                    break;
                default:
                    Debug.LogError("Unexpected enum value: " + request.ToString());
                    throw new Exception();
            }

            if (acc == null)
            {
                response.Result = false;
                response.FailReason = GetAccountDataResponse.Error.AccountNotFound;
                connection.Connection.Response(response);
                return;
            }

            var s = request.Settings;

            response.AccountData = new();
            response.AccountData.AccountID = acc.AccountID;

            if (s.GetPublicInfo)
            {
                response.Info.HavePublicInfo = acc.PublicData != null;

                if (response.Info.HavePublicInfo)
                    response.AccountData.PublicData = acc.PublicData;
                else if (s.Mode == DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing)
                    goto Responsing;
            }

            if (s.GetLogInData)
            {
                response.Info.HaveLogInData = acc.SecureData != null && acc.SecureData.LogInData != null;

                if (response.Info.HaveLogInData)
                    response.AccountData.SecureData.LogInData = acc.SecureData.LogInData;
                else if (s.Mode == DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing)
                    goto Responsing;
            }

            if (s.GetSecureHistory)
            {
                response.Info.HaveSecureHistory = acc.SecureData != null && acc.SecureData.History != null;

                if (response.Info.HaveSecureHistory)
                    response.AccountData.SecureData.History = acc.SecureData.History;
                else if (s.Mode == DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing)
                    goto Responsing;
            }

            if (s.GetTemporary)
            {
                response.Info.HaveTemporary = acc.Temporary != null;

                if (response.Info.HaveTemporary)
                    response.AccountData.Temporary = acc.Temporary;
                else if (s.Mode == DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing)
                    goto Responsing;
            }

            if (s.GetGameStatistics)
            {
                response.Info.HaveGameStatistics = acc.GameStatistics != null;

                if (response.Info.HaveGameStatistics)
                {
                    if (s.IncludeGameHistory)
                    {
                        response.AccountData.GameStatistics = acc.GameStatistics;
                    }
                    else
                    {
                        response.AccountData.GameStatistics.Wins = acc.GameStatistics.Wins;
                        response.AccountData.GameStatistics.Total = acc.GameStatistics.Total;
                        response.AccountData.GameStatistics.Mmr = acc.GameStatistics.Mmr;
                    }
                }
                else if (s.Mode == DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing)
                    goto Responsing;

            }

        Responsing:

            response.Result = response.Info.Result(s);

            if (!response.Result)
                response.FailReason = GetAccountDataResponse.Error.RequestedEntitiesMissing;

            connection.Connection.Response(response);
        }

        private void HandleAdminConnectionError(ResponsingConnection obj)
        {
            obj.Connection.Dispose();
            _adminConnections.Remove(obj);
        }

        private void ParseAllowedIps()
        {
            _allowedIps = new IPAddress[_allowedIpStrings.Length];
            for (int i = 0; i < _allowedIpStrings.Length; i++)
            {
                _allowedIps[i] = IPAddress.Parse(_allowedIpStrings[i]);
            }
        }
    }
}