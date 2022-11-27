using DevourDev.Base.Security;
using DevourDev.MonoBase;
using DevourDev.Networking;
using FD.Global.Sides;
using FD.Networking;
using FD.Networking.Realm.GamePackets;
using FD.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FD.Global
{
    public class RealmPlayersConnector : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _eventsDisplayText;

        [Tooltip("in milliseconds"), SerializeField] private int _timeout = 1000;


        private ConnectionsHandlerBase _connectionsHandler;
        private GameManager _gm;
        private FD_RealmGamePacketsResolver _realmPacketsResolver;

        private List<TmpResponsingConnection> _guests;
        private Dictionary<long, TestPlayerConnectionObjectConstructor> _constructings;

        private SecurityHandler _securer = new();
        private readonly object _connectToRealmLocker = new();


        public Socket Socket => _connectionsHandler.MainSocket;

        private void OnDestroy()
        {

            if (_connectionsHandler != null)
            {
                _connectionsHandler.OnNewConnection -= HandleConnection;
                ((IDisposable)_connectionsHandler).Dispose();
            }

            foreach (var g in _guests)
            {
                ((IDisposable)g).Dispose();
            }

            foreach (var c in _constructings)
            {
                ((IDisposable)c.Value).Dispose();
            }
        }

        private void Awake()
        {

        }

        private void Start()
        {
            _realmPacketsResolver = new FD_RealmGamePacketsResolver();
            _gm = GameManager.Instance;
            _guests = new();
            _constructings = new();
            //_connectionsHandler.OnNewConnection += (s) => UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            //{
            //    HandleConnection(s);
            //});
        }

        public int Init()
        {
            if (_connectionsHandler != null)
            {
                Debug.LogError($"Attempt to Init inited ({nameof(_connectionsHandler)} != null)");
                throw new Exception($"Attempt to Init inited ({nameof(_connectionsHandler)} != null)");
            }
            _connectionsHandler = new(0, 5);

            return ((IPEndPoint)_connectionsHandler.MainSocket.LocalEndPoint).Port;
        }

        public void StartHandling()
        {
            _connectionsHandler.OnNewConnection += HandleConnection;
            _connectionsHandler.StartAccepting();
        }
        public void StopHandling()
        {
            _connectionsHandler.CancelAccepting();
        }


        private void HandleConnection(Socket handler)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = $"New connection: {handler.RemoteEndPoint}";
                _eventsDisplayText.text = msg;
                Debug.Log(msg);
            });
            TmpResponsingConnection g = new(handler, 4096, _timeout,
            new Action<TmpResponsingConnection, string>(RejectGuestConnection),
            new Action<IPacketContent, TmpResponsingConnection>(HandleGuestRequest),
            _realmPacketsResolver);
            g.Handle();
        }

        private void HandleGuestRequest(IPacketContent content, TmpResponsingConnection connection)
        {
            _guests.Remove(connection);
            switch (content)
            {
                case ConnectToRealmRequest cReq:
                    lock (_connectToRealmLocker)
                    {
                        HandleConnectToRealmRequest(cReq, connection);
                    }
                    break;
                default:
                    UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                    {
                        string msg = "Unknown Packet - rejecting...";
                        _eventsDisplayText.text = msg;
                        Debug.Log(msg);
                    });
                    RejectGuestConnection(connection, $"HandleGuestRequest: unexpected request: {content.GetType()}");
                    break;
            }
        }

        private void HandleConnectToRealmRequest(ConnectToRealmRequest req, TmpResponsingConnection connection)
        {
            var response = new ConnectToRealmResponse();

            //check key length (should be 2048)
            if (req.Key.Length != SecurityHandler.DEFAULT_SECURITY_LEVEL)
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    string msg = $"CONNECT_TO_REALM_REQUEST:: key length - FAILURE! Length: {req.Key.Length}";
                    _eventsDisplayText.text = msg;
                    Debug.LogError(msg);
                });

                response.Result = false;
                response.FailReason = ConnectToRealmResponse.Error.WrongSessionKey;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = "CONNECT_TO_REALM_REQUEST:: key length - OK!";
                _eventsDisplayText.text = msg;
                Debug.Log(msg);
            });

            //decode accountID (last 8 bytes)
            long accID = SecurityHandler.GetEncodedValue(req.Key);

            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = $"CONNECT_TO_REALM_REQUEST:: acc ID - {accID}";
                _eventsDisplayText.text = msg;
                Debug.Log(msg);
            });

            if (req.ConnectionType != ConnectionType.MessagesListener && req.ConnectionType != ConnectionType.Requester)
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    string msg = $"CONNECT_TO_REALM_REQUEST:: connection type - FAILURE! Type: {req.ConnectionType}";
                    _eventsDisplayText.text = msg;
                    Debug.LogError(msg);
                });
                response.Result = false;
                response.FailReason = ConnectToRealmResponse.Error.InvalidConnectionTypeRequest;
                connection.Connection.Response(response);
                goto RejectConnection;
            }

            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = $"CONNECT_TO_REALM_REQUEST:: connection type - {req.ConnectionType}";
                _eventsDisplayText.text = msg;
                Debug.Log(msg);
            });

            //check if Constructings contains this ID as key
            if (_constructings.TryGetValue(accID, out var constructing))
            {
                //if yes - compare keys, add new socket and it will try to evolve to ready-to-go connection
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    string msg = $"CONNECT_TO_REALM_REQUEST:: constructing  EXISTS for accID {accID}";
                    _eventsDisplayText.text = msg;
                    Debug.Log(msg);
                });
                if (constructing.CompareKeys(req.Key))
                {
                    switch (req.ConnectionType)
                    {
                        case ConnectionType.Requester:
                            constructing.SetResponsingHandler(connection.Connection.Socket);
                            break;
                        case ConnectionType.MessagesListener:
                            constructing.SetMessagingHandler(connection.Connection.Socket);
                            break;
                        default:
                            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                            {
                                string msg = $"Unexpected switch statement : {req.ConnectionType}";
                                _eventsDisplayText.text = msg;
                                Debug.LogError(msg);
                            });
                            response.Result = false;
                            response.FailReason = ConnectToRealmResponse.Error.InvalidConnectionTypeRequest;
                            connection.Connection.Response(response);
                            goto RejectConnection;
                    }

                    response.Result = true;
                    connection.Connection.Response(response);
                    return;
                }
                else
                {
                    UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                    {
                        string msg = $"CONNECT_TO_REALM_REQUEST:: constructing.CompareKeys - FAILURE!";
                        _eventsDisplayText.text = msg;
                        Debug.LogError(msg);
                    });
                    response.Result = false;
                    response.FailReason = ConnectToRealmResponse.Error.WrongSessionKey;
                    connection.Connection.Response(response);
                    goto RejectConnection;
                }
            }
            else
            {
                //if not - create new
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    string msg = $"CONNECT_TO_REALM_REQUEST:: constructing NOT EXISTS for accID {accID}";
                    _eventsDisplayText.text = msg;
                    Debug.Log(msg);
                });
                var constructor = new TestPlayerConnectionObjectConstructor(accID, req.Key,
                    new Action<TestPlayerConnectionObjectConstructor, string>(RejectConstructing),
                    new Action<TestPlayerConnectionObjectConstructor>(Init), _timeout);
                _constructings.Add(accID, constructor);

                switch (req.ConnectionType)
                {
                    case ConnectionType.Requester:
                        constructor.SetResponsingHandler(connection.Connection.Socket);
                        break;
                    case ConnectionType.MessagesListener:
                        constructor.SetMessagingHandler(connection.Connection.Socket);
                        break;
                    default:
                        UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                        {
                            string msg = $"Unexpected switch statement : {req.ConnectionType}";
                            _eventsDisplayText.text = msg;
                            Debug.LogError(msg);
                        });
                        response.Result = false;
                        response.FailReason = ConnectToRealmResponse.Error.Other;
                        goto RejectConnection;
                }

                response.Result = true;
                connection.Connection.Response(response);
                return;

            }

        RejectConnection:

            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = "Rejecting connection from HandleConnectToGardenRequest...";
                _eventsDisplayText.text = msg;
                Debug.LogError(msg);
            });
            RejectGuestConnection(connection, response.FailReason.ToString());
        }

        private void RejectGuestConnection(TmpResponsingConnection connection, string error)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = $"Rejecting GuestConnection: {error}";
                _eventsDisplayText.text = msg;
                Debug.LogError(msg);
            });
            _guests.Remove(connection);
            connection.Dispose();
        }
        private void RejectConstructing(TestPlayerConnectionObjectConstructor c, string error)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = $"Constructing rejected: {error}";
                _eventsDisplayText.text = msg;
                Debug.LogError(msg);
            });
            _constructings.Remove(c.AccID);
            c.Dispose();
        }
        private void Init(TestPlayerConnectionObjectConstructor c)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string msg = "Adding User Object...";
                _eventsDisplayText.text = msg;
                Debug.LogError(msg);
            });
            ResponsingConnection rc = new(c.ResponsingSocket, 1024, _realmPacketsResolver);
            MessagingConnection mc = new(c.MessagingSocket);

            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                var gs = _gm.AddGameSide<GameSideOnline>();
                var rsm = FD.Networking.Realm.RealmServerManager.Instance;
                int playerIndex = -1;
                for (int i = 0; i < rsm.Settings.PlayersKeys.Length; i++)
                {
                    if (c.AccID == rsm.Settings.PlayersIDs[i])
                    {
                        playerIndex = i;
                        break;
                    }
                }
                if(playerIndex == -1)
                {
                    UnityMainThreadDispatcher.InvokeOnMainThread(() => Debug.LogError("Unable to find player index (RealmPlayersConnector)"));
                    return;
                }
                gs.Init(new(rc, mc, c.AccID), rsm.Settings.PlayersIDs[playerIndex], rsm.Settings.PlayersMmrs[playerIndex]);
                var pginfo = new PlayerGameInfo(c.AccID, new int[] { 0, 1, 2 });
                gs.SetPlayerInfo(pginfo);
                var npo = gs.NetworkPlayerObject;
                npo.OnError += (x) => //todo: do))) (see GameSideOnline.HandleNetworkError)
                {
                    _gm.DisconnectOnlineSide(gs);
                    UnityMainThreadDispatcher.InvokeOnMainThread(() => Debug.LogError("Error окурок"));
                    npo.Dispose();
                };
                npo.StartHandling();

                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    string msg = "User Object added!";
                    _eventsDisplayText.text = msg;
                    Debug.LogError(msg);
                });

                _constructings.Remove(c.AccID);
            });

        }


    }
}