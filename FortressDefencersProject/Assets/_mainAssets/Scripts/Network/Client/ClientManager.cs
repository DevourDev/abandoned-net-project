using DevourDev.Networking;
using System.Threading.Tasks;
using UnityEngine;
using FD.Networking.Garden.Packets;
using FD.Networking.Gates.Packets;
using FD.Networking.Realm.GamePackets;
using FD.Networking.Packets;
using DevourDev.MonoExtentions;
using FD.Networking.Realm;
using TMPro;

namespace FD.Networking.Client
{
    public class ClientManager : MonoBehaviour
    {
        public event System.Action<IPacketContent, ListeningConnection> OnGardenMessageReceived;
        //public event System.Action<IPacketContent> OnGardenResponseReceived;
        public event System.Action<IPacketContent> OnRealmMessageReceived;
        public event System.Action<IPacketContent> OnRealmResponseReceived;

        public const int BUFFER_SIZE = 1_048_576;

        public static ClientManager Instance { get; private set; }
        private bool _singletonedSuccessfully;

        [SerializeField] private UnityIpEndPoint _gatesUIpEP;
        [SerializeField] private UnityIpEndPoint _gardenUIpEP;

        [SerializeField] private TextMeshProUGUI _bytesInGardenChannel;
        [SerializeField] private GameObject _bytesInGardenCanvas;

        private void Start()
        {
            DontDestroyOnLoad(_bytesInGardenCanvas);
        }

        private void Update()
        {
            if (_gardenConnection == null || _gardenConnection.Channel == null
                || !_gardenConnection.Channel.Connected)
            {
                _bytesInGardenChannel.text = "Garden Connection is null";
                return;
            }

            string t = $"available:\nreq: {_gardenConnection.Channel.RequestingConnection.Connection.Socket.Available}\n" +
                $"lis:{_gardenConnection.Channel.ListeningConnection.Connection.Socket.Available}";
            _bytesInGardenChannel.text = t;
        }


        private RequestingConnection _gatesConnection;
        private GardenConnection _gardenConnection;
        private RealmConnection _realmConnection;

        private WC_GatesPacketsResolver _gatesPacketsResolver;
        private FD_GardenPacketsResolver _gardenPacketsResolver;


        public bool ConnectedToGarden
        {
            get
            {
                try
                {
                    if (_gardenConnection == null)
                        return false;

                    return _gardenConnection.Channel.RequestingConnection.Connection.Socket.Connected
                        && _gardenConnection.Channel.ListeningConnection.Connection.Socket.Connected;
                }
                catch (System.NullReferenceException)
                {
                    return false;
                }
                catch (System.ObjectDisposedException)
                {
                    return false;
                }
            }
        }


        public WC_GatesPacketsResolver GatesPacketsResolver => _gatesPacketsResolver;
        public FD_GardenPacketsResolver GardenPacketsResolver => _gardenPacketsResolver;


        public RequestingConnection GatesConnection => _gatesConnection;
        public GardenConnection GardenConnection => _gardenConnection;
        public RealmConnection RealmConnection => _realmConnection;


        private void OnDestroy()
        {
            return;
            try
            {
                if (ConnectedToGarden)
                {
                    RequestGarden(new DisconnectRequest()); //just to wait until response received. Asynchronous is not needed
                }
            }
            catch (System.Exception) { }

            try
            {
                _gatesConnection?.Connection.Dispose();
            }
            catch (System.Exception) { }

            try
            {
                _gardenConnection?.Channel?.Dispose();
            }
            catch (System.Exception) { }

            try
            {
                _realmConnection?.Dispose();
            }
            catch (System.Exception) { }
        }

        private void Awake()
        {
            InitSingleton(true, true);
            if (!_singletonedSuccessfully)
                return;
            _gatesPacketsResolver = new WC_GatesPacketsResolver();
            _gardenPacketsResolver = new FD_GardenPacketsResolver();
            //_realmConnection = new();                                            // to delete
            //_realmConnection.OnRealmMessageReceived += HandleRealmMessage;       // to delete
            //_realmConnection.OnRealmResponseReceived += HandleRealmResponse;     // to delete
        }

        private void RealmChannel_OnError(RealmConnection obj)
        {
            try
            {
                _realmConnection.Dispose();
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                _realmConnection = null;
            }
        }

        private void HandleRealmResponse(IPacketContent p)
        {
            switch (p) //handle system response
            {
                default: //leave handling other responses to other handlers
                    OnRealmResponseReceived?.Invoke(p);
                    break;
            }
        }

        private void HandleRealmMessage(IPacketContent p)
        {
            switch (p) //handle system messages
            {
                default: //leave handling other messages to other handlers
                    OnRealmMessageReceived?.Invoke(p);
                    break;
            }
        }

        private async Task ConnectToGatesServer()
        {
            if (_gatesConnection != null)
            {
                _gatesConnection.Connection.Dispose();
            }

            var s = await DevNet.ConnectToAsync(_gatesUIpEP.GetIPEndPoint());
            _gatesConnection = new RequestingConnection(s, BUFFER_SIZE, _gatesPacketsResolver);
            _gatesConnection.Connection.OnError += GatesConnection_OnError;
            
        }

        private void GatesConnection_OnError(NetworkConnection obj)
        {
            this.InvokeOnMainThread(() =>
            {
                Debug.Log($"Error in {nameof(ClientManager)} with Gates connection. Error: {obj.LastException}");

                Debug.Log("Gates connection will be disposed.");

                obj.Dispose();
                _gatesConnection = null;
            });
        }

        public async Task<bool> TryConnectToRealmAsync(System.Net.IPEndPoint ep, byte[] realmKey)
        {
            //todo: check if already connected to realm
            _realmConnection = new();
            _realmConnection.OnRealmMessageReceived += HandleRealmMessage;
            _realmConnection.OnRealmResponseReceived += HandleRealmResponse;
            _realmConnection.SetIpep(ep);
            this.InvokeOnMainThread(() => Debug.LogError($"_realmConnection.SetIpep(ep). ep = {ep.Address}:{ep.Port}"));
            _realmConnection.SetKey(realmKey);
            this.InvokeOnMainThread(() => Debug.LogError($"_realmConnection.SetKey(realmKey). realmKey.Length, [0][1][2] = {realmKey.Length}, [{realmKey[0]}][{realmKey[1]}][{realmKey[2]}]"));
            return await _realmConnection.TryConnect(RealmChannel_OnError);
        }

        public async Task<bool> TryConnectToGardenServerAsync(byte[] sessionKey)
        {
            if (_gardenConnection != null)
            {
                if (ConnectedToGarden)
                {
                    this.InvokeOnMainThread(() => Debug.LogError("Already connected to Garden."));
                    return true;
                }
                else
                {
                    this.InvokeOnMainThread(() => Debug.LogError($"Not connected to Garden, but {nameof(_gardenConnection)} != null. Disposing..."));
                    _gardenConnection.Channel?.Dispose();

                }
            }


            NetworkConnection requestingNC = new(BUFFER_SIZE);
            NetworkConnection listeningNC = new(BUFFER_SIZE);

            await requestingNC.ConnectAsync(_gardenUIpEP.GetIPEndPoint());
            Garden.Packets.ConnectToGardenRequest ctgrAsRequester = new();
            ctgrAsRequester.SessionKey = sessionKey;
            ctgrAsRequester.ConnectionType = ConnectionType.Requester;
            var waitingForReqConResponse = requestingNC.RequestAsync(ctgrAsRequester, _gardenPacketsResolver);

            var reqConResponse = await waitingForReqConResponse;

            if (reqConResponse is not ConnectToGardenResponse reqRes)
            {
                Debug.LogError("Requesting connection: received damaged request.");
                return false;
            }
            if (!reqRes.Result)
            {
                Debug.LogError("Requesting connection request was denied.");
                return false;
            }

            await listeningNC.ConnectAsync(_gardenUIpEP.GetIPEndPoint());
            Garden.Packets.ConnectToGardenRequest ctgrAsListener = new();
            ctgrAsListener.SessionKey = sessionKey;
            ctgrAsListener.ConnectionType = ConnectionType.MessagesListener;
            var waitingForLisConResponse = listeningNC.RequestAsync(ctgrAsListener, _gardenPacketsResolver);

            var lisConResponse = await waitingForLisConResponse;

            if (lisConResponse is not ConnectToGardenResponse lisRes)
            {
                Debug.LogError("Listening connection: received damaged request.");
                return false;
            }
            if (!lisRes.Result)
            {
                Debug.LogError("Listening connection request was denied.");
                return false;
            }


            Debug.Log("Successfully connected to Garden!");

            var reqCon = new RequestingConnection(requestingNC, _gardenPacketsResolver);
            var lisCon = new ListeningConnection(listeningNC, _gardenPacketsResolver);

            var lrChannel = new ListeningRequesterChannel(reqCon, lisCon);

            _gardenConnection = new GardenConnection(lrChannel);
            _gardenConnection.SetSessionKey(sessionKey);
            _gardenConnection.Channel.OnError += GardenConnection_OnError;
            _gardenConnection.Channel.ListeningConnection.OnMessageReceived += ListeningConnection_OnMessageReceived;
            _gardenConnection.Channel.ListeningConnection.StartListening();


            Debug.Log("Garden Connection inited.");

            return true;
        }

        private void GardenConnection_OnError(ListeningRequesterChannel obj)
        {
            var dtn = System.DateTime.Now.Ticks;
            this.InvokeOnMainThread(() =>
            {
                Debug.Log($"{dtn}: Error in {nameof(ClientManager)} with Garden connection. Error: {obj.LastException}");

                Debug.Log("Garden connection will be disposed.");

                obj.Dispose();
                _gardenConnection = null;
            });
        }
        private async void ListeningConnection_OnMessageReceived(IPacketContent p, ListeningConnection c)
        {
            switch (p)
            {
                case HeartBeatRequest hbreq:
                    this.InvokeOnMainThread(() =>
                    {
                        Debug.LogError("Heartbeat Message Received");
                    });
                    await _gardenConnection.Channel.RequestingConnection.SendRequestAsync(new HeartBeatResponse());
                    break;
                default:
                    OnGardenMessageReceived?.Invoke(p, c);
                    break;
            }
        }

        public async Task SendRequestToRealm(IPacketContent content)
        {
            //todo: check for realm connection
            //todo: add sync (task-locker-waiter) to not request something with waiting for response
            // while (_waitingForRealmResponse)
            // {
            // Wait();
            // }
            // _waitingForRealmResponse = true;
            await _realmConnection.Channel.RequestingConnection.SendRequestAsync(content);
            // _waitingForRealmResponse = false;
        }

        public async Task<IPacketContent> RequestRealm(IPacketContent c)
        {
            return await _realmConnection.Channel.RequestingConnection.RequestAsyncLocking(c);
        }

        public async Task<IPacketContent> RequestGates(IPacketContent content)
        {
            await ConnectToGatesServer();
            return await _gatesConnection.RequestAsyncLocking(content);
        }

        public async Task<IPacketContent> RequestGarden(IPacketContent content)
        {
            if (!ConnectedToGarden)
            {
                Debug.LogError("Not connected to Garden!");
                return null;
            }

            IPacketContent response = null;

            response = await _gardenConnection.Channel.RequestingConnection.RequestAsyncLocking(content);

            return response;

            //for (int i = 0; i < 5; i++)
            //{
            //    try
            //    {
            //        response = await _gardenConnection.Channel.RequestingConnection.RequestAsync(content);
            //        break;
            //    }
            //    catch (System.Exception ex)
            //    {
            //        Debug.LogError($"error in RequestGarden trying to send {content.GetType()} request and receive response. Error: {ex}");
            //    }

            //    if (!await TryConnectToGardenServerAsync(_gardenConnection.SessionKey))
            //    {
            //        break;
            //    }
            //}

            //if (response == null)
            //{
            //    Debug.LogError("Failed to request Garden. Check you intýrnet connection.");
            //    System.Threading.Tasks.Task.Delay(5000).Wait();
            //    Application.Quit(228);
            //}

            //return response;

        }

        //RequestRealm

        private void InitSingleton(bool destroyOnFailure = true, bool dontDestroyOnLoadOnSuccess = false)
        {
            if (Instance == this)
            {
                goto Success;
            }

            if (Instance == null)
            {
                Instance = this;
                goto Success;
            }

            _singletonedSuccessfully = false;
            if (destroyOnFailure)
            {
                Destroy(gameObject);
            }
            return;


        Success:
            _singletonedSuccessfully = true;
            if (dontDestroyOnLoadOnSuccess)
            {
                DontDestroyOnLoad(gameObject);
            }
            return;
        }

    }
}