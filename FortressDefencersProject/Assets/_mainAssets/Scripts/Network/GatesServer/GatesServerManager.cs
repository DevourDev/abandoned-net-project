using DevourDev.Base.Security;
using DevourDev.Base.SystemExtentions;
using DevourDev.MonoBase;
using DevourDev.Networking;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using FD.Networking.Database.Packets;
using FD.Networking.Gates.Packets;

namespace FD.Networking.Gates
{

    public class GatesServerManager : MonoBehaviour
    {
        [SerializeField] private int _port = 6666;
        [SerializeField] private int _maxConnectionsQ = 3;

        [SerializeField] private UnityIpEndPoint _databaseUIpEP;

        private ConnectionsHandlerBase _guestsConnectionsHandler;
        private FD_DatabasePacketsResolver _databasePacketsResolver;
        private WC_GatesPacketsResolver _gatesPacketsResolver;

        private RequestingConnection _databaseChannel;

        private List<TmpResponsingConnection> _guests;
        private readonly object _guestsHandlingLocker = new();

        private void OnApplicationQuit()
        {
            _guestsConnectionsHandler?.Dispose();
            _databaseChannel?.Connection.Dispose();

            foreach (var g in _guests)
            {
                g.Dispose();
            }
        }
        private void Start()
        {
            _databasePacketsResolver = new();
            _gatesPacketsResolver = new();

            _guests = new(128);

            _guestsConnectionsHandler = new(_port, _maxConnectionsQ);
            _guestsConnectionsHandler.OnNewConnection += HandleConnectedGuest;

            ConnectToDatabaseServer();
            _guestsConnectionsHandler.StartAccepting();
        }

        private void ConnectToDatabaseServer()
        {
            var s = DevNet.ConnectTo(_databaseUIpEP.GetIPEndPoint());
            _databaseChannel = new(s, 1024 * 32, _databasePacketsResolver, true);

            Debug.Log("Connected to Database");
        }

        private void HandleConnectedGuest(Socket handler)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() => { Debug.Log("New Connection! (from dispatcher)"); });
            TmpResponsingConnection g = new(handler, 512, 1000,
                new Action<TmpResponsingConnection, string>(RejectGuestConnection),
                new Action<IPacketContent, TmpResponsingConnection>(HandleGuestRequest),
                _gatesPacketsResolver);
            g.Handle();
        }

        private void HandleGuestRequest(IPacketContent content, TmpResponsingConnection connection)
        {
            // lock (_guestsHandlingLocker) //ÍÀÕÓß?)))
            // {
            UnityMainThreadDispatcher.InvokeOnMainThread(() => { Debug.Log("Incoming request from GUEST (from dispatcher)"); });
            switch (content)
            {
                case Packets.LogInRequest liReq:
                    HandleLogInRequest(liReq, connection);
                    break;
                case Packets.SignUpRequest suReq:
                    HandleSignUpRequest(suReq, connection);
                    break;
                default:
                    RejectGuestConnection(connection, $"HandleGuestRequest: unexpected request: {content.GetType()}");
                    break;
            }

            //RejectGuestConnection(connection, "");
            // }
        }

        private void HandleSignUpRequest(SignUpRequest request, TmpResponsingConnection connection)
        {
            var response = new SignUpResponse();

            if (!CheckLogin(request.Login))
            {
                response.Result = false;
                response.FailReason = SignUpResponse.Error.BadLogin;
                goto Responsing;
            }

            if (!CheckPasswords(request.ClientSideHashedPassword, request.ClientSideHashedPasswordConfirmation))
            {
                response.Result = false;
                response.FailReason = SignUpResponse.Error.WrongPasswords;
                goto Responsing;
            }

            if (!Database.Entities.Account.EmailAddress.TryParse(request.Email, out var parsedEmail))
            {
                response.Result = false;
                response.FailReason = SignUpResponse.Error.BadEmail;
                goto Responsing;
            }

            var dbReq = new Database.Packets.HandleSignUpRequest();
            dbReq.Login = request.Login;
            dbReq.ReHashedPassword = Hasher.HashString(request.ClientSideHashedPassword);
            dbReq.Email = parsedEmail;

            var anonDbRes = _databaseChannel.Request(dbReq);

            if (anonDbRes is not HandleSignUpResponse dbRes)
            {
                response.Result = false;
                response.FailReason = SignUpResponse.Error.Other;
                goto Responsing;
            }

            if (!dbRes.Result)
            {
                response.Result = false;
                response.FailReason = dbRes.FailReason switch
                {
                    HandleSignUpResponse.Error.UnavailableLogin => SignUpResponse.Error.NonUniqueLogin,
                    _ => SignUpResponse.Error.Other,
                };
                goto Responsing;
            }

            response.Result = true;
            response.SessionKey = dbRes.SessionKey;
            goto Responsing;


Responsing:

            connection.Connection.Response(response);
            RejectGuestConnection(connection, $"connection handled and released (ok)");


            bool CheckPasswords(string p1, string p2)
            {
                return p1 == p2;
            }
        }

        private static bool CheckLogin(string login)
        {
            return LengthIsValid() && SymbolsAreAllowed();


            bool LengthIsValid() => 3 < login.Length && login.Length < 16; //BloodTrail SyntaxSugar
            bool SymbolsAreAllowed() => !login.ContainsAny('!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+');
        }



        private void HandleLogInRequest(LogInRequest request, TmpResponsingConnection connection)
        {
            var response = new LogInResponse();

            if (!CheckLogin(request.Login))
            {
                response.Result = false;
                response.FailReason = LogInResponse.Error.WrongLogin;
                goto Responsing;
            }

            var dbReq = new Database.Packets.HandleLogInRequest();
            dbReq.Login = request.Login.ToLower();
            dbReq.ReHashedPassword = Hasher.HashString(request.ClientSideHashedPassword);

            var anonDbRes = _databaseChannel.Request(dbReq);

            if (anonDbRes is not HandleLogInResponse dbRes)
            {
                response.Result = false;
                response.FailReason = LogInResponse.Error.Other;
                goto Responsing;
            }

            if (!dbRes.Result)
            {
                response.Result = false;
                response.FailReason = dbRes.FailReason switch
                {
                    HandleLogInResponse.Error.AccountNotFound => LogInResponse.Error.WrongLogin,
                    HandleLogInResponse.Error.WrongPassword => LogInResponse.Error.WrongPassword,
                    _ => LogInResponse.Error.Other,
                };
                goto Responsing;
            }

            response.Result = true;
            response.SessionKey = dbRes.SessionKey;
            goto Responsing;


Responsing:

            connection.Connection.Response(response);
            RejectGuestConnection(connection, $"connection handled and released (ok)");
        }

        private void RejectGuestConnection(TmpResponsingConnection connection, string error)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Debug.LogError($"Rejecting guest connection: {error}");
            });
            _guests.Remove(connection);
            connection.Dispose();
        }


    }
}
