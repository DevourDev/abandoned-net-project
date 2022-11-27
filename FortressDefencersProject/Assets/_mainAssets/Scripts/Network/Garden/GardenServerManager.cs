using DevourDev.MonoBase;
using DevourDev.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using FD.Networking;
using FD.Networking.Database.Packets;
using FD.Networking.Garden.Packets;
using FD.Networking.Gates.Packets;
using DevourDev.MonoExtentions;
using FD.Networking.Realm;
using System.Security.Cryptography;
using DevourDev.Base;

namespace FD.Networking.Garden
{
    public class GardenServerManager : MonoBehaviour
    {
        [SerializeField, Tooltip("in seconds")] private int _timeToAcceptGame = 30;


        public event Action<string> OnVisualLog;
        public event Action<IPacketContent> OnRequestReceived;
        public event Action<int> OnOnlinersCountChanged;
        public event Action<int> OnLobbiesCountChanged;

        [SerializeField] private int _clientsPort = 7777;
        [SerializeField] private int _maxConnectionsQ = 10;
        [SerializeField] private ApplicationLauncherDefault _realmLauncher;
        [SerializeField] private int _heartBeatInterval = 10_000;
        [SerializeField] private UnityIpEndPoint _databaseUIpEP;

        private int _realmsPort;

        private ConnectionsHandlerBase _guestsConnectionsHandler;
        private ConnectionsHandlerBase _realmsConnectionsHandler;
        private FD_DatabasePacketsResolver _databasePacketsResolver;
        private FD_GardenPacketsResolver _gardenPacketsResolver;
        private GardenRealmPacketsResolver _gardenRealmPacketsResolver;

        private RequestingConnection _databaseChannel;

        private List<TmpResponsingConnection> _guests;
        /// <summary>
        /// Signed up users in progress of connection.
        /// </summary>
        private Dictionary<long, GardenUserObjectConstructor> _constructings;
        /// <summary>
        /// Properly connected users.
        /// </summary>
        private Dictionary<long, GardenUserObject> _onlineUsers;
        private readonly object _onlinePlayersLocker = new();
        private HashSet<long> _usersToDisconnect;
        private CancellationTokenSource _heartBeatCts;
        private int _heartBeatBlyat;

        private MatchMaker _matchMaker;
        private readonly System.Random _r = new();
        private readonly DevourDev.Base.Security.SecurityHandler _securer = new();

        /// <summary>
        /// long - match ID
        /// </summary>
        private Dictionary<long, GardenRealmConnection> _constructingRealms;
        private Dictionary<long, GardenRealmConnection> _freeRealms;
        private Dictionary<long, GardenRealmConnection> _activeRealms;
        private Dictionary<long, Lobby> _lobbies;
        private long _nextLobbyID;

        private void OnApplicationQuit()
        {
            _databaseChannel?.Connection?.Dispose();
            _guestsConnectionsHandler?.Dispose();
            foreach (var g in _guests)
            {
                try
                {
                    g.Dispose();
                }
                catch (Exception)
                {

                }
            }
            foreach (var c in _constructings)
            {
                try
                {
                    c.Value?.Dispose();
                }
                catch (Exception)
                {

                }
            }
            var onlineUsersIDs = _onlineUsers.Keys.ToArray();
            for (int i = 0; i < onlineUsersIDs.Length; i++)
            {
                long ouID = onlineUsersIDs[i];
                DisconnectOnlineUser(ouID);
            }

            foreach (var rc in _freeRealms)
            {
                try
                {
                    rc.Value?.Dispose();
                }
                catch (Exception)
                {

                }
            }
            foreach (var rc in _activeRealms)
            {
                try
                {
                    rc.Value?.Dispose();
                }
                catch (Exception)
                {

                }
            }
            foreach (var rc in _constructingRealms)
            {
                try
                {
                    rc.Value?.Dispose();
                }
                catch (Exception)
                {

                }
            }
        }

        private void StartHeartBeating() //todo: serverside should not send "requests" to clients. Clients should send requests every x seconds.
        {
            //return;
            _heartBeatCts = new();
            var token = _heartBeatCts.Token;
            Task.Factory.StartNew(() =>
            {
                DateTime lastCheck = DateTime.Now;
                DateTime nextCheck;
                while (!token.IsCancellationRequested)
                {
                    nextCheck = lastCheck + TimeSpan.FromMilliseconds(_heartBeatInterval);
                    lock (_onlinePlayersLocker)
                    {
                        foreach (var ou in _onlineUsers)
                        {
                            try
                            {
                                var delta = (lastCheck - ou.Value.LastHeartBeat).TotalMilliseconds;
                                if (delta < 0)
                                {
                                    this.InvokeOnMainThread(() =>
                                    {
                                        Debug.LogError("lastCheck - LastHeartBeat = " + delta.ToString());
                                    });

                                    if (!_usersToDisconnect.Contains(ou.Key))
                                    {
                                        _usersToDisconnect.Add(ou.Key);
                                    }
                                    continue;
                                }
                            }
                            catch (SocketException sex)
                            {
                                this.InvokeOnMainThread(() =>
                                {
                                    Debug.LogError("sex: " + sex);
                                });

                                if (!_usersToDisconnect.Contains(ou.Key))
                                {
                                    _usersToDisconnect.Add(ou.Key);
                                }
                                continue;
                            }


                            ou.Value.CheckConnectionState();
                        }


                        foreach (var id in _usersToDisconnect)
                        {
                            DisconnectOnlineUser(id);
                        }
                        _usersToDisconnect.Clear();

                    }

                    lastCheck = nextCheck;
                    var waitTime = nextCheck - DateTime.Now;
                    if (waitTime.TotalMilliseconds > 0)
                        Task.Delay(waitTime).Wait();
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

        private void DisconnectOnlineUser(long id)
        {
            var u = _onlineUsers[id];
            if (u.InFindGameQ)
            {
                _matchMaker.RemovePlayerFromQueue(id);
            }
            if (u.InFoundGameLobby)
            {
                u.FoundGameLobby.SetNotReady(id);
                u.FoundGameLobby.Destroy();
            }
            if (u.InRealmGameLobby)
            {
                u.RealmLobby.SetNotReady(id);
            }
            _onlineUsers.Remove(id);
            u.Dispose();
            OnOnlinersCountChanged?.Invoke(_onlineUsers.Count);
        }

        private void Start()
        {
            _databasePacketsResolver = new();
            _gardenPacketsResolver = new();
            _gardenRealmPacketsResolver = new();

            _guests = new(128);
            _constructings = new(128);
            _onlineUsers = new(128);
            _lobbies = new();
            _nextLobbyID = 0;
            _usersToDisconnect = new();
            _constructingRealms = new();
            _freeRealms = new();
            _activeRealms = new();

            _guestsConnectionsHandler = new(_clientsPort, _maxConnectionsQ);
            _guestsConnectionsHandler.OnNewConnection += HandleConnectedGuest;
            _realmsConnectionsHandler = new(0, _maxConnectionsQ);
            _realmsPort = ((System.Net.IPEndPoint)_realmsConnectionsHandler.MainSocket.LocalEndPoint).Port;

            Debug.LogError("Realms port: " + _realmsPort);
            _realmsConnectionsHandler.OnNewConnection += HandleConnectedRealm;

            _matchMaker = new();
            _matchMaker.OnFinishTeam += MatchMaker_OnFinishTeam;

            ConnectToDatabaseServer();

            _guestsConnectionsHandler.StartAccepting();
            _realmsConnectionsHandler.StartAccepting();
            _heartBeatBlyat = _heartBeatInterval * 2;
            StartHeartBeating();
            // Debug.Log("Garden: end of Start()");
        }

        #region Connection
        private void ConnectToDatabaseServer()
        {
            var s = DevNet.ConnectTo(_databaseUIpEP.GetIPEndPoint());
            _databaseChannel = new(s, 1024 * 32, _databasePacketsResolver, true);
        }

        #region Guests

        private void HandleConnectedGuest(Socket handler)
        {
            //Debug.Log("GARDEN: Guest connected.");
            TmpResponsingConnection g = new(handler, 512, 1000,
                new Action<TmpResponsingConnection, string>((c, error) => RejectGuestConnection(c, error)),
                new Action<IPacketContent, TmpResponsingConnection>(HandleGuestRequest),
                _gardenPacketsResolver);
            g.Handle();
        }
        private void HandleConnectedRealm(Socket handler)
        {
            Debug.LogError("GARDEN: Realm connected.");
            TmpResponsingConnection g = new(handler, 1024 * 3, 1000,
                 new Action<TmpResponsingConnection, string>((c, error) => RejectGuestConnection(c, error)),
                new Action<IPacketContent, TmpResponsingConnection>(HandleGuestRealmRequest),
                _gardenRealmPacketsResolver);
            g.Handle();
        }

        private void HandleGuestRealmRequest(IPacketContent content, TmpResponsingConnection connection)
        {
            OnRequestReceived?.Invoke(content);

            switch (content)
            {
                case InitRealmGardenConnectionRequest cReq:
                    HandleInitRealmGardenConnectionRequest(cReq, connection);
                    break;
                default:
                    //UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                    //{
                    //    Debug.Log("Unknown Packet - rejecting...");
                    //});
                    RejectGuestConnection(connection, $"HandleGuestRealmRequest: unexpected request: {content.GetType()}");
                    break;
            }
        }

        private void HandleInitRealmGardenConnectionRequest(InitRealmGardenConnectionRequest cReq, TmpResponsingConnection guestConnection)
        {
            long realmID = cReq.RealmID;

            if (!_constructingRealms.TryGetValue(realmID, out var con))
            {
                OnVisualLog?.Invoke($"REALM_GARDEN_CONNECTION: declined: wrong realm ID.");
                RejectGuestConnection(guestConnection, "wrong realm id");
                return;
            }

            if (!con.CompareKeys(cReq.GardenKey))
            {
                OnVisualLog?.Invoke($"REALM_GARDEN_CONNECTION: declined: wrong garden key.");
                RejectGuestConnection(guestConnection, "wrong garden key");
                return;
            }

            OnVisualLog?.Invoke($"before con.InitConnection");
            var res = con.InitConnection(guestConnection.Connection.Socket);
            OnVisualLog?.Invoke($"after con.InitConnection. Result: {res}");

            if (!res)
            {
                OnVisualLog?.Invoke($"REALM_GARDEN_CONNECTION: declined: error in GardenRealmConnection.InitConnection(s).");
                RejectGuestConnection(guestConnection, "error in GardenRealmConnection.InitConnection(s)");
                return;
            }

            _constructingRealms.Remove(realmID);
            _activeRealms.Add(realmID, con);


            con.OnRequestReceived += RealmConnection_OnRequestReceived;
            con.Responsing.OnError += (x) => RealmConnection_OnError(realmID, con);

            var response = new InitRealmGardenConnectionResponse();
            response.Result = true;
            con.Response(response);


            OnVisualLog?.Invoke($"HandleInitRealmGardenConnectionRequest handled.");
        }

        private void RealmConnection_OnError(long id, GardenRealmConnection c)
        {
            OnVisualLog?.Invoke($"Error in RealmConnection {DateTime.Now.ToShortTimeString()}");
            _activeRealms.Remove(id);
            try
            {
                c.Dispose();
            }
            catch (Exception)
            {
            }


        }

        private void RealmConnection_OnRequestReceived(IPacketContent p, GardenRealmConnection c)
        {
            OnRequestReceived?.Invoke(p);

            switch (p)
            {
                case TestGardenRealmRequest test:
                    HandleRealmTestRequest(test, c);
                    break;
                case PlayerLostRequest playerLostRequest:
                    HandlePlayerLostRequest(playerLostRequest, c);
                    break;
                case PlayerWonRequest playerWonRequest:
                    HandlePlayerWonRequest(playerWonRequest, c);
                    break;
                //case GameOverRequest gameOverReq:
                //    HandleGameOverRequest(gameOverReq, c);
                //    break;
                case CloseRealmRequest closeRealmReq:
                    HandleCloseRealmRequest(closeRealmReq, c);
                    break;
                default:
                    break;
            }
        }

        private void HandlePlayerLostRequest(PlayerLostRequest playerLostRequest, GardenRealmConnection c)
        {
            var changeMmrRequest = new RegistrateGameOverRequest();
            changeMmrRequest.AccID = playerLostRequest.LostPlayerID;
            changeMmrRequest.MatchID = c.UniqueID;
            changeMmrRequest.MatchResult = MatchResult.Lose;
            changeMmrRequest.MmrChange = -playerLostRequest.MmrToRemove;
            var rawRes = _databaseChannel.Request(changeMmrRequest);
            PlayerLostResponse response = new();
            if (rawRes is not RegistrateGameOverResponse res || !res.Result)
            {
                response.Result = false;
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"Unexpected response from DataBase. {nameof(RegistrateGameOverResponse)} expected. {rawRes.GetType()} received (or !result).");
                });
            }
            else
            {
                response.Result = true;
            }

            c.Response(response);

        }

        private void HandlePlayerWonRequest(PlayerWonRequest playerWonRequest, GardenRealmConnection c)
        {
            var changeMmrRequest = new RegistrateGameOverRequest
            {
                AccID = playerWonRequest.WinnerPlayerID,
                MatchID = c.UniqueID,
                MatchResult = MatchResult.Lose,
                MmrChange = playerWonRequest.MmrToAdd,
            };
            var rawRes = _databaseChannel.Request(changeMmrRequest);
            PlayerWonResponse response = new();
            if (rawRes is not RegistrateGameOverResponse res || !res.Result)
            {
                response.Result = false;
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"Unexpected response from DataBase. {nameof(RegistrateGameOverResponse)} expected. {rawRes.GetType()} received (or !result).");
                });
            }
            else
            {
                response.Result = true;
            }

            c.Response(response);
        }

        private void HandleGameOverRequest(GameOverRequest gameOverReq, GardenRealmConnection con)
        {
            //add match data to matches database

            GameOverResponse res = new();
            res.Result = true;
            res.CloseRealm = true;
            con.Response(res);

            con.Lobby.Destroy();
        }

        private void HandleCloseRealmRequest(CloseRealmRequest closeRealmReq, GardenRealmConnection connection)
        {
            var res = new CloseRealmResponse();
            res.Result = true;
            connection.Response(res);
        }

        private void HandleRealmTestRequest(TestGardenRealmRequest test, GardenRealmConnection c)
        {
            var response = new TestGardenRealmResponse();
            OnVisualLog?.Invoke($"int: {test.SomeInt}, Vector3: {test.SomeVector3}");

            response.Result = _r.Next(0, 2) > 0;
            c.Response(response);
        }

        private void HandleGuestRequest(IPacketContent content, TmpResponsingConnection connection)
        {
            OnRequestReceived?.Invoke(content);

            switch (content)
            {
                case Packets.ConnectToGardenRequest ctgReq:
                    HandleConnectToGardenRequest(ctgReq, connection);
                    break;
                default:
                    //UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                    //{
                    //    Debug.Log("Unknown Packet - rejecting...");
                    //});
                    RejectGuestConnection(connection, $"unexpected HandleGuestRequest content switch: {content.GetType()}");
                    break;
            }
        }


        private void HandleConnectToGardenRequest(ConnectToGardenRequest request, TmpResponsingConnection connection)
        {
            var response = new ConnectToGardenResponse();

            if (request.SessionKey == null || request.SessionKey.Length != Database.Entities.Account.EKey.EKEY_SIZE)
            {
                response.Result = false;
                response.FailReason = ConnectToGardenResponse.Error.Other;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            if (request.ConnectionType != ConnectionType.MessagesListener && request.ConnectionType != ConnectionType.Requester)
            {
                response.Result = false;
                response.FailReason = ConnectToGardenResponse.Error.Other;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            long accID = BitConverter.ToInt64(request.SessionKey, Database.Entities.Account.EKey.EKEY_SIZE - 8);

            if (_constructings.TryGetValue(accID, out var foundC))
            {
                if (foundC.CompareKeys(request.SessionKey))
                {
                    switch (request.ConnectionType)
                    {
                        case ConnectionType.Requester:
                            foundC.SetResponsingHandler(connection.Connection.Socket);
                            break;
                        case ConnectionType.MessagesListener:
                            foundC.SetMessagingHandler(connection.Connection.Socket);
                            break;
                        default:
                            Debug.LogError("Unexpected switch statement.");
                            goto RejectConnection;
                    }

                    response.Result = true;
                    connection.Connection.Response(response);
                    return;
                }
                else
                {
                    //RejectConstructing(_constructings[accID]);
                    goto RejectConnection;
                }
            }

            var databaseTmpDataRequest = Database.Packets.GetAccountDataRequest.GetTmpData(accID);
            var databaseAnonResponse = _databaseChannel.Request(databaseTmpDataRequest);

            if (databaseAnonResponse is not GetAccountDataResponse tmpDataResponse)
            {
                response.Result = false;
                response.FailReason = ConnectToGardenResponse.Error.Other;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            if (!tmpDataResponse.Result)
            {
                response.Result = false;
                response.FailReason = ConnectToGardenResponse.Error.Other;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            if (!tmpDataResponse.AccountData.Temporary.TryGetKey(Database.Entities.Account.EKeyType.GardenSession, out var k))
            {
                response.Result = false;
                response.FailReason = ConnectToGardenResponse.Error.Other;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            if (!k.KeyEqual(request.SessionKey))
            {
                response.Result = false;
                response.FailReason = ConnectToGardenResponse.Error.WrongSessionKey;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            response.Result = true;
            var constructor = new GardenUserObjectConstructor(k.Key, accID,
                              new Action<GardenUserObjectConstructor, string>(RejectConstructing),
                              new Action<GardenUserObjectConstructor>(AddUserObject), 1000_000);
            _constructings.Add(accID, constructor);

            switch (request.ConnectionType)
            {
                case ConnectionType.Requester:
                    constructor.SetResponsingHandler(connection.Connection.Socket);
                    break;
                case ConnectionType.MessagesListener:
                    constructor.SetMessagingHandler(connection.Connection.Socket);
                    break;
                default:
                    constructor.Dispose();
                    this.InvokeOnMainThread(() => Debug.LogError("Unexpected switch statement."));
                    goto RejectConnection;
            }


            connection.Connection.Response(response);
            return;

        RejectConnection:

            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Debug.Log("Rejecting connection from HandleConnectToGardenRequest...");
            });
            RejectGuestConnection(connection, "");
        }


        private void RejectGuestConnection(TmpResponsingConnection connection, string error)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Debug.Log($"Guest connection rejected: {error}");
            });
            _guests.Remove(connection);
            connection.Dispose();
        }

        #endregion

        #region Constructings
        private void RejectConstructing(GardenUserObjectConstructor c, string error)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Debug.Log($"Constructing rejected: {error}");
            });
            _constructings.Remove(c.AccID);
            c.Dispose();
        }
        #endregion

        #region Users

        private void AddUserObject(GardenUserObjectConstructor c)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Debug.Log("Adding User Object...");
            });
            long id = c.AccID;
            ResponsingConnection rc = new(c.ResponsingSocket, 1024, _gardenPacketsResolver);
            MessagingConnection mc = new(c.MessagingSocket);
            var newUser = new GardenUserObject(rc, mc, id);
            newUser.OnError += HandleUserError;
            newUser.OnRequestReceived += HandleUserRequest;
            _onlineUsers.Add(id, newUser);
            _constructings.Remove(id);
            newUser.StartHandling();
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Debug.LogError($"User Object added! ID: {id}");
            });
            OnOnlinersCountChanged?.Invoke(_onlineUsers.Count);
        }

        private void HandleUserError(GardenUserObject obj)
        {
            var dtn = System.DateTime.Now.Ticks;
            this.InvokeOnMainThread(() =>
            {

                Debug.Log($"{dtn}: Error in {nameof(GardenServerManager)}, {nameof(HandleUserError)} : {obj.Responsing.Connection.LastException}");
            });
            try
            {
                DisconnectOnlineUser(obj.AccID);
            }
            catch (Exception)
            {

            }
        }

        private void HandleUserRequest(IPacketContent content, GardenUserObject userObject)
        {
            OnRequestReceived?.Invoke(content);

            switch (content)
            {
                case Packets.TestGardenEchoRequest echoReq:
                    HandleTestEchoRequest(echoReq, userObject);
                    break;

                case DisconnectRequest disconnectRequest:
                    HandleDisconnectRequest(disconnectRequest, userObject);
                    break;

                case EnterFindGameQueueRequest efgReq:
                    HandleEnterFindGameQueueRequest(efgReq, userObject);
                    break;

                case QuitFindGameQueueRequest qfgReq:
                    HandleQuitFindGameQueueRequest(qfgReq, userObject);
                    break;

                case AcceptGameRequest acgr:
                    HandleAcceptGameRequest(acgr, userObject);
                    break;

                case GetMyProfileDataRequest gmpdReq:
                    HandleGetMyProfileDataRequest(gmpdReq, userObject);
                    break;

                case OnlinePlayersStatsRequest onlinePlayersStatsRequest:
                    HandleOnlinePlayersStatsRequest(onlinePlayersStatsRequest, userObject);
                    break;

                default:
                    OnVisualLog?.Invoke($"Unexpected request: {content.GetType().Name} from user ID {userObject.AccID}.");
                    HandleUserError(userObject);
                    return;
            }
        }

        private void HandleOnlinePlayersStatsRequest(OnlinePlayersStatsRequest onlinePlayersStatsRequest, GardenUserObject userObject)
        {
            OnlinePlayersStatsResponse res = new();

            res.Result = true;
            res.OnlinersCount = _onlineUsers.Count;
            res.SearchersCount = _matchMaker.QueueCount;

            userObject.Response(res);
        }

        private void HandleQuitFindGameQueueRequest(QuitFindGameQueueRequest qfgReq, GardenUserObject userObject)
        {
            QuitFindGameQueueResponse res = new();

            if (!userObject.InFindGameQ || userObject.InFoundGameLobby || userObject.InRealmGameLobby)
            {
                res.Result = false;
                userObject.Response(res);
                return;
            }

            OnVisualLog?.Invoke($"User with ID {userObject.AccID} removing from MM queue");
            _matchMaker.RemovePlayerFromQueue(userObject.AccID);
            userObject.InFindGameQ = false;

            res.Result = true;
            userObject.Response(res);
            return;
        }

        private void HandleEnterFindGameQueueRequest(EnterFindGameQueueRequest efgReq, GardenUserObject userObject)
        {
            EnterFindGameQueueResponse res = new();
            var preferences = new GetAccountDataRequestSettings()
            {
                GetGameStatistics = true,
                GetLogInData = false,
                GetSecureHistory = false,
                GetTemporary = false,
                IncludeGameHistory = false,
                Mode = DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing,
            };
            if (!userObject.TryGetActualAccount(preferences, out Database.Entities.Account.Account acc))
            {
                if (!TryGetAccount(userObject.AccID, out acc, preferences))
                {
                    res.Result = false;
                    res.FailReason = EnterFindGameQueueResponse.Error.Other;
                    userObject.Response(res);
                    OnVisualLog?.Invoke($"Unable to get AccData for User with ID {userObject.AccID}.");
                    return;
                }
            }

            res.Result = true;
            userObject.Response(res);

            OnVisualLog?.Invoke($"User with ID {userObject.AccID} adding to MM queue");
            _matchMaker.AddPlayerToQueue(acc);
            userObject.InFindGameQ = true;

        }

        private void HandleGetMyProfileDataRequest(GetMyProfileDataRequest gmpdReq, GardenUserObject userObject)
        {
            bool accountFound = TryGetAccount(userObject.AccID, out var acc, new GetAccountDataRequestSettings { GetPublicInfo = true, GetGameStatistics = true });
            var res = new GetMyProfileDataResponse();

            if (!accountFound)
            {
                Debug.LogError($"Account was not found for ID {userObject.AccID}");
                res.Result = false;
                userObject.Response(res);
                return;
            }

            res.AccID = userObject.AccID;
            res.MatchesCount = acc.GameStatistics.Total;
            res.Mmr = acc.GameStatistics.Mmr;
            res.Wins = acc.GameStatistics.Wins;
            res.NickName = acc.PublicData.NickName;
            res.Result = true;
            userObject.Response(res);


        }

        private void HandleAcceptGameRequest(AcceptGameRequest acgr, GardenUserObject userObject)
        {
            try
            {
                if (!userObject.InFoundGameLobby)
                    return;

                if (userObject.AcceptedFoundGame)
                    return;

                var response = new AcceptGameResponse();

                if (userObject.FoundGameDate + TimeSpan.FromSeconds(_timeToAcceptGame) > DateTime.Now)
                {
                    OnVisualLog?.Invoke($"User with ID {userObject.AccID} is ready in Found Game Lobby.");
                    userObject.FoundGameLobby.SetReady(userObject.AccID);
                    response.Result = true;
                }
                else
                {
                    OnVisualLog?.Invoke($"User with ID {userObject.AccID} gets Find Game ban (10 sec).");
                    userObject.AddTempBanTimeDuration(TimeSpan.FromSeconds(10));
                    response.Result = false;
                    response.FailReason = AcceptGameResponse.Error.Timeout;
                }

                userObject.Response(response);
            }
            catch (Exception ex)
            {
                this.InvokeOnMainThread(() => Debug.LogError($"HandleAcceptGame({acgr}, {userObject}) exception: {ex}"));
                DisconnectOnlineUser(userObject.AccID);
            }

        }

        private void HandleDisconnectRequest(DisconnectRequest disconnectRequest, GardenUserObject userObject)
        {
            var responce = new DisconnectResponse();
            userObject.Response(responce);
            DisconnectOnlineUser(userObject.AccID);
        }

        private bool TryGetAccount(long id, out Database.Entities.Account.Account acc, GetAccountDataRequestSettings settings = null)
        {

            if (settings == null)
            {
                settings = new GetAccountDataRequestSettings();
                settings.GetGameStatistics = true;
                settings.Mode = DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing;
            }

            if (_onlineUsers.TryGetValue(id, out var guo))
            {
                if (guo.TryGetActualAccount(settings, out acc))
                {
                    return true;
                }
            }

            FD.Networking.Database.Packets.GetAccountDataRequest req = new();
            req.SearchMode = Database.AccountSearchMode.ByID;
            req.Settings = settings;
            req.AccountID = id;
            var rawResponse = _databaseChannel.Request(req);
            if (rawResponse is GetAccountDataResponse accRes && accRes.Result)
            {
                acc = accRes.AccountData;
                _onlineUsers[id].SetAccount(acc, settings);
                return true;
            }

            acc = null;
            return false;

        }


        private void HandleTestEchoRequest(TestGardenEchoRequest request, GardenUserObject userObject)
        {

            Debug.Log("TestGardenEchoRequest received from " + userObject.Responsing.Connection.Socket.RemoteEndPoint + " \n" +
                "Content: " + request.Message);

            TestGardenEchoResponse response = new();
            response.Message = "ECHO: " + request.Message;

            userObject.Response(response);
            UnityMainThreadDispatcher.InvokeOnMainThread(() => StartCoroutine(TestMessageSender(userObject)));
        }

        private IEnumerator TestMessageSender(GardenUserObject userObject)
        {

            Debug.Log("Entered IEnumerator");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(1, 5));
            TestGardenPacketMessage m = new();
            m.Message = DevourDev.Base.Generators.StringGenerator.FastGenerateEnglishString(UnityEngine.Random.Range(10, 15));

            userObject.Messaging.Message(m);
        }

        #endregion
        #endregion


        #region MatchMaking

        private void MatchMaker_OnFinishTeam(long[] competitonsIDs)
        {
            OnVisualLog?.Invoke($"MatchMaker_OnFinishTeam: players count: {competitonsIDs.Length} first: {competitonsIDs[0]}, second {competitonsIDs[1]}.");
            GameFoundMessage gfm = new();
            gfm.FoundGameMode = GameMode.Default;

            Lobby l = new(_nextLobbyID);
            _lobbies.Add(_nextLobbyID, l);
            _nextLobbyID++;
            l.OnLobbyDestroyed += HandleOnFoundGameLobbyDestroyed;
            l.OnAllPlayersReady += FoundGameLobby_OnAllPlayersReady;

            var now = DateTime.Now;
            for (int i = 0; i < competitonsIDs.Length; i++)
            {
                if (!l.TryAddPlayer(competitonsIDs[i]))
                {
                    this.InvokeOnMainThread(() => UnityEngine.Debug.Log($"Player ID {competitonsIDs[i]} is already exists in Lobby."));
                    continue;
                }
                _onlineUsers[competitonsIDs[i]].InFindGameQ = false;
                _onlineUsers[competitonsIDs[i]].FoundGameLobby = l;
                _onlineUsers[competitonsIDs[i]].Messaging.Connection.Message(gfm);
            }
        }

        private void HandleOnFoundGameLobbyDestroyed(Lobby l)
        {
            _lobbies.Remove(l.UniqueID);
            OnLobbiesCountChanged?.Invoke(_lobbies.Count);

            var lobbyDestroyedMsg = new LobbyDestroyedMessage();

            l.IterateAllPlayers((id, rdy) =>
            {
                if (_onlineUsers.TryGetValue(id, out var guo))
                {
                    guo.FoundGameLobby = null;
                    guo.Messaging.Message(lobbyDestroyedMsg);
                }
            }, out var ex);

            if (ex != null)
            {
                OnVisualLog?.Invoke($"HandleOnFoundGameLobbyDestroyed - iterating caused exception: {ex.Message}");
            }
        }

        private async void FoundGameLobby_OnAllPlayersReady(Lobby l)
        {
            OnLobbiesCountChanged?.Invoke(_lobbies.Count);
            OnVisualLog?.Invoke("Lobby created! " + DateTime.Now.ToShortTimeString());

            //send players WaitForRealmExecutionMessage
            var gettingRealm = GetRealm();
            byte[][] playerKeys = new byte[2][];
            List<long> playersIDs = new(); // implement as array
            l.IterateAllPlayers((playerID, r) => playersIDs.Add(playerID), out var exception);
            if (exception != null)
                OnVisualLog?.Invoke($"Error in iterating (776): {exception.Message}");

            OnVisualLog?.Invoke($"FoundGameLobby_OnAllPlayersReady: iterating all players returns {playersIDs.Count} items.");
            GardenUserObject[] guos = new GardenUserObject[playersIDs.Count];
            for (int i = 0; i < guos.Length; i++)
            {
                guos[i] = _onlineUsers[playersIDs[i]];
            }
            for (int i = 0; i < playerKeys.Length; i++)
            {
                playerKeys[i] = _securer.GenerateKey(playersIDs[i]);
            }
            var realmCon = await gettingRealm;
            OnVisualLog?.Invoke("xxx before StartHandling");
            for (int i = 0; i < guos.Length; i++)
            {
                var prefs = new GetAccountDataRequestSettings()
                {
                    GetGameStatistics = true,
                    GetLogInData = false,
                    GetSecureHistory = false,
                    GetTemporary = false,
                    IncludeGameHistory = false,
                    Mode = DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing,
                };
                if (!guos[i].TryGetActualAccount(prefs, out var acc))
                {
                    if (!TryGetAccount(playersIDs[i], out acc, prefs))
                    {
                        OnVisualLog?.Invoke("TryGetAccount FAILED in GardenServerManager.FoundGameLobby_OnAllPlayersReady()");
                        return;
                    }
                }
            }
            int[] playersMmrs = new int[playersIDs.Count];
            //int[][] playersDecks = new int[playersIDs.Count][];
            for (int i = 0; i < playersIDs.Count; i++)
            {
                playersMmrs[i] = guos[i].Account.GameStatistics.Mmr;
                //add playersDecks from acc
            }
            await realmCon.StartHandling(playerKeys, new int[][] { new int[] { 0 }, new int[] { 0 } }, playersMmrs);
            OnVisualLog?.Invoke("xxx after StartHandling");


            int j = 0;
            l.IterateAllPlayers((id, rds) =>
            {
                if (_onlineUsers.TryGetValue(id, out var gardenUserObject))
                {
                    var realmInviteMsg = new RealmInviteMessage();
                    realmInviteMsg.RealmIPEP = realmCon.RealmIpepForPlayers;
                    realmInviteMsg.RealmSessionKey = playerKeys[j];
                    gardenUserObject.Messaging.Message(realmInviteMsg);
                    j++;
                }
            }, out var ex);

            if (ex != null)
                OnVisualLog?.Invoke($"Error in iterating: {ex.Message}");

            OnVisualLog?.Invoke($"j value: {j}");

        }
        #endregion

        private async Task<GardenRealmConnection> GetRealm()
        {
            long realmID = -1;
            if (_freeRealms.Count == 0)
            {
                realmID = ExecuteNewRealm();
            }

            GardenRealmConnection realmConnection = null;
            OnVisualLog?.Invoke("Waiting for Realm Initializing started.");
            while (true)
            {
                await Task.Delay(1000);
                if (_activeRealms.TryGetValue(realmID, out realmConnection))
                    break;
            }

            OnVisualLog?.Invoke("Realm Connection awaited.");

            return realmConnection;

        }

        private long _nextRealmID; //todo: get this value from Database as Last Game ID + 1 (and registrate it to increment immediately) (or not)
                                   //(90% sure - not) (better create Dictionary<long, long> for MatchID and RealmID for active...)
        private long ExecuteNewRealm()
        {
            OnVisualLog?.Invoke("New Realm Execution started.");
            long realmID = _nextRealmID;
            _nextRealmID++;
            byte[] gardenKey = _securer.GenerateKey(realmID);
            // byte[] gardenKey = new byte[] {0,0,0};
            string pathToKeyFile = _realmLauncher.GetRootPathForNewFile(realmID.ToString());
            var writen = FileHandler.WriteFile(pathToKeyFile, gardenKey);
            Debug.Log("Key-file writen: " + writen.ToString());
            var realmCon = new GardenRealmConnection(gardenKey, _gardenRealmPacketsResolver);
            _constructingRealms.Add(realmID, realmCon);
            string exargs = $"--- garden_port === {_realmsPort} -valend --- key_file_path === {pathToKeyFile} -valend --- realm_id === {realmID} -valend";
            _realmLauncher.LaunchApp(exargs);

            return realmID;
        }
    }


}
