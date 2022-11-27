using DevourDev.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using FD.Networking;
using DevourDev.Networking;
using DevourDev.MonoExtentions;
using System.IO;
using System.Threading.Tasks;
using DevourDev.Base.Security;

namespace FD.Networking.Realm
{
    public class InitRealmGardenConnectionRequest : IPacketContent
    {
        public int UniqueID => 100;
        public long RealmID;
        /// <summary>
        /// Key that Realm gets from Garden via Execution Args (path to file with)
        /// </summary>
        public byte[] GardenKey;
        /// <summary>
        /// Key, generated at Realm
        /// </summary>
        //public byte[] RealmKey;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            RealmID = d.ReadInt64();
            GardenKey = d.ReadBytes();
            // RealmKey = d.ReadBytes();

        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(RealmID);
            e.Write(GardenKey);
            //  e.Write(RealmKey);
        }
    }
    public class InitRealmGardenConnectionResponse : IPacketContent
    {
        public int UniqueID => 101;
        public bool Result;


        public InitRealmGardenConnectionResponse()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }

    public class UsersInitialDataRequest : IPacketContent
    {
        public int UniqueID => 200;
        public int RealmPortForPlayers;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            RealmPortForPlayers = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(RealmPortForPlayers);
        }
    }

    public class UsersInitialDataResponse : IPacketContent
    {
        public int UniqueID => 201;
        public bool Result;
        public int[][] PlayersDecks;
        public byte[][] PlayersEnterRealmKeys;
        public int[] PlayersMmrs;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (!Result)
                return;

            PlayersDecks = d.ReadIntArrays();
            PlayersEnterRealmKeys = d.ReadByteArrays();
            PlayersMmrs = d.ReadInts();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (!Result)
                return;

            e.Write(PlayersDecks);
            e.Write(PlayersEnterRealmKeys);
            e.Write(PlayersMmrs);
        }
    }

    public class TestGardenRealmRequest : IPacketContent
    {
        public int UniqueID => 500;
        public int SomeInt;
        public Vector3 SomeVector3;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SomeInt = d.ReadInt();
            SomeVector3 = d.ReadVector3();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SomeInt);
            e.Write(SomeVector3);
        }
    }
    public class TestGardenRealmResponse : IPacketContent
    {
        public int UniqueID => 501;
        public bool Result;
        public int SomeInts;
        public string SomeString;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (Result)
                SomeInts = d.ReadInt();
            else
                SomeString = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (Result)
                e.Write(SomeInts);
            else
                e.Write(SomeString);
        }
    }
    public class PlayerLostRequest : IPacketContent
    {
        public int UniqueID => 510;
        public long LostPlayerID;
        public int MmrToRemove;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            LostPlayerID = d.ReadInt64();
            MmrToRemove = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(LostPlayerID);
            e.Write(MmrToRemove);
        }
    }
    public class PlayerLostResponse : IPacketContent
    {
        public int UniqueID => 511;
        public bool Result;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }
    public class PlayerWonRequest : IPacketContent
    {
        public int UniqueID => 520;
        public long WinnerPlayerID;
        public int MmrToAdd;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            WinnerPlayerID = d.ReadInt64();
            MmrToAdd = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(WinnerPlayerID);
            e.Write(MmrToAdd);
        }
    }
    public class PlayerWonResponse : IPacketContent
    {
        public int UniqueID => 521;
        public bool Result;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }
    public class GameOverRequest : IPacketContent
    {
        public int UniqueID => 530;


        public GameOverRequest()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }
    public class GameOverResponse : IPacketContent
    {
        public int UniqueID => 531;
        public bool Result;
        public bool CloseRealm;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();

            if (!Result)
                return;

            CloseRealm = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);

            if (!Result)
                return;

            e.Write(CloseRealm);
        }
    }
    public class CloseRealmRequest : IPacketContent
    {
        public int UniqueID => 10_000;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }
    public class CloseRealmResponse : IPacketContent
    {
        public int UniqueID => 10_001;
        public bool Result;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }
    public class GardenRealmPacketsResolver : Networking.Packets.FD_PacketsResolverBase
    {
        public override IPacketContent GetSpecialPacket(int packetID)
        {
            return packetID switch
            {
                100 => new InitRealmGardenConnectionRequest(),
                101 => new InitRealmGardenConnectionResponse(),

                200 => new UsersInitialDataRequest(),
                201 => new UsersInitialDataResponse(),

                500 => new TestGardenRealmRequest(),
                501 => new TestGardenRealmResponse(),

                510 => new PlayerLostRequest(),
                511 => new PlayerLostResponse(),

                520 => new PlayerWonRequest(),
                521 => new PlayerWonResponse(),

                530 => new GameOverRequest(),
                531 => new GameOverResponse(),

                10_000 => new CloseRealmRequest(),
                10_001 => new CloseRealmResponse(),
                _ => throw new NotImplementedException("Unexpected packet ID: " + packetID),
            };

        }
    }
    public class RealmServerManager : MonoBehaviour
    {
        [SerializeField] private string _gardenIpString;
        [SerializeField] FD.Global.RealmPlayersConnector _realmConnector_prtp;

        //[SerializeField] private string _fixedRealmKeyString;
        //[SerializeField] private string _executionArgsPrefix = "--";
        // [SerializeField] private string _executionArgsPointer = "==";

        private Dictionary<string, string> _executionArgs;
        private RealmSettings _settings;
        private RequestingConnection _gardenChannel;
        private GardenRealmPacketsResolver _gardenRealmPacketsResolver;

        private System.Random _r;
        public static RealmServerManager Instance { get; private set; }
        private bool _singletonedSuccessfully;


        public RealmSettings Settings => _settings;
        public RequestingConnection GardenChannel => _gardenChannel;


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
        private void Awake()
        {
            InitSingleton();
            if (!_singletonedSuccessfully)
                return;

            _r = new();
            _gardenRealmPacketsResolver = new GardenRealmPacketsResolver();

        }

        [SerializeField] private string[] _pseudoArgs;

        private void Start()
        {
            if (!_singletonedSuccessfully)
                return;
            Global.GameManager.Instance.OnStartGame += GM_OnStartGame;
            Global.GameManager.Instance.OnEndGame += GM_OnEndGame;

            //_executionArgs = new();
            //for (int i = 0; i < _pseudoArgs.Length; i++)
            //{
            //    _executionArgs.Add(_pseudoArgs[i], _pseudoArgs[i + 1]);
            //    i++;
            //}

            _executionArgs = FileHandler.GetDevourArgsAsDictionary();
            Debug.Log($"Execution args detected: {_executionArgs.Count}");
            foreach (var kvp in _executionArgs)
            {
                Debug.Log($"key: {kvp.Key}, value: {kvp.Value}");
            }
            //Debug.Log

            foreach (var ea in _executionArgs)
            {

            }
            Debug.Log($"Execution args detected: {_executionArgs.Count}");
            _settings = new();
            HandleExArgs();
            InitRealm();
        }

        private void GM_OnEndGame(int exitCode)
        {

            Debug.Log("REALM_SERVER_MANAGER:: OnEndGame event handled (empty)");
        }

        private void GM_OnStartGame(int exitCode)
        {
            // _realmConnector_prtp.StartHandling //if player disconnects, he cannot reconnect this way.
            //todo: OnStartGame stop listeting MainSocket (of connector)
            // on some player disconnect - resume listening



        }

        private void HandleExArgs()
        {
            if (_executionArgs.TryGetValue("garden_port", out var port))
            {
                Debug.LogError("Garden port received: " + port);
                _settings.GardenPort = int.Parse(port);
            }
            else
            {
                Debug.LogError("Garden port NOT received");
            }

            if (_executionArgs.TryGetValue("key_file_path", out var keyFilePath))
            {
                Debug.LogError("Key file path received: " + keyFilePath);
                ParseKeyFile(keyFilePath);
            }
            else
            {
                Debug.LogError("Key file path NOT received");
            }

            if (_executionArgs.TryGetValue("realm_id", out var realmIdString))
            {
                Debug.LogError("Realm ID received: " + realmIdString);
                _settings.RealmID = long.Parse(realmIdString);
            }
            else
            {
                Debug.LogError("Realm ID NOT received");
            }

            return;
            foreach (var argPair in _executionArgs)
            {
                switch (argPair.Key)
                {
                    //case "game mode":
                    //    _settings.GameMode = argPair.Item2;
                    //    break;
                    case "garden_port":
                        _settings.GardenPort = int.Parse(argPair.Value);
                        Debug.LogError("Garden port received: " + argPair.Value);
                        break;
                    case "key_file_path":
                        ParseKeyFile(argPair.Value);
                        break;
                    case "realm_id":
                        _settings.RealmID = long.Parse(argPair.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ParseKeyFile(string item2)
        {
            //_settings.GardenKey = new byte[] { 0, 0, 0 };
            //return;
            Debug.Log("Trying to get key from file: " + item2);
            var fs = new FileStream(item2, FileMode.Open, FileAccess.Read);
            byte[] key = new byte[fs.Length];
            fs.Read(key, 0, key.Length);
            _settings.GardenKey = key;
            Debug.Log("Key found. Length: " + key.Length);
        }

        private async void InitRealm()
        {
            InitPlayersConnector();
            await ConnectToGarden();
            StartHandlingPlayersConnector();
            //SendPlayersStatesToGarden();
            //StartGameSession();
        }

        private void InitPlayersConnector()
        {
            _settings.RealmPort = _realmConnector_prtp.Init();
        }

        private void StartHandlingPlayersConnector()
        {
            _realmConnector_prtp.StartHandling();

            Debug.LogError("OKOKOKOKOK!!!!!!!!!!!!!");
        }


        private async Task RequestInitialPlayersDataToGarden()
        {
            var req = new UsersInitialDataRequest();
            req.RealmPortForPlayers = _settings.RealmPort;

            this.InvokeOnMainThread(() =>
            {
                Debug.LogError($"REALM_SERVER:: RequestInitialPlayersDataToGarden()");
            });

            var rawRes = await _gardenChannel.RequestAsync(req);

            if (rawRes is not UsersInitialDataResponse res)
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"{nameof(UsersInitialDataRequest)} requested, {rawRes.GetType().Name} received. (Unexpected)");
                });
                return;
            }

            if (!res.Result)
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"{nameof(UsersInitialDataRequest)} requested, {rawRes.GetType().Name} received. {nameof(res.Result)} is false.");
                });
                return;
            }

            SetPlayersData(res);

            this.InvokeOnMainThread(() =>
            {
                Debug.LogError($"REALM_SERVER:: RequestInitialPlayersDataToGarden() finished!");
            });
        }

        private async Task ConnectToGarden()
        {
            var s = await DevNet.ConnectToAsync(new IPEndPoint(IPAddress.Parse(_gardenIpString), _settings.GardenPort));
            this.InvokeOnMainThread(() =>
            {
                Debug.LogError($"{nameof(ConnectToGarden)}: 1");
            });
            _gardenChannel = new RequestingConnection(s, 1024 * 32, _gardenRealmPacketsResolver);
            _gardenChannel.Connection.OnError += GardenChannel_OnError; ;
            InitRealmGardenConnectionRequest creq = new();
            creq.RealmID = _settings.RealmID;
            creq.GardenKey = _settings.GardenKey;
            var rawResponse1 = await _gardenChannel.RequestAsync(creq);
            this.InvokeOnMainThread(() =>
            {
                Debug.LogError($"{nameof(ConnectToGarden)}: 2"); // logged!
            });

            if (rawResponse1 is not InitRealmGardenConnectionResponse initResponse)
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"rawResponse1 is not InitRealmGardenConnectionResponse");
                });
                return;
            }

            if (!initResponse.Result)
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"Connection response is failure.");
                });
                return;
            }

            this.InvokeOnMainThread(() =>
            {

                Debug.LogError("CONNECTED TO GARDEN SUCCESSFULLY!"); //logged
            });


            await RequestInitialPlayersDataToGarden();

        }

        private void GardenChannel_OnError(NetworkConnection obj)
        {
            this.InvokeOnMainThread(() =>
            {
                Debug.LogError($"Error in {nameof(RealmServerManager)} in Garden connection. Error: {obj.LastException}");
                Debug.LogError($"Garden connection will be disposed.");
                obj.Dispose();
                _gardenChannel = null;
            });
        }



        private void SetPlayersData(UsersInitialDataResponse data)
        {
            this.InvokeOnMainThread(() => Debug.Log("users_initial_data received..."));
            this.InvokeOnMainThread(() =>
            {

                Debug.LogError($"users_initial_data received: \n" +
                    $"decks_0: {data.PlayersDecks[0]}\n" +
                    $"decks_1: {data.PlayersDecks[1]}\n" +
                    $"decks length: {data.PlayersDecks.Length}\n" +
                    $"enter_realm_key_0: {data.PlayersEnterRealmKeys[0]}\n" +
                     $"enter_realm_key_1: {data.PlayersEnterRealmKeys[1]}\n");
            });

            _settings.PlayersKeys = data.PlayersEnterRealmKeys;
            int playersCount = _settings.PlayersKeys.Length;
            _settings.PlayersIDs = new long[playersCount];
            for (int i = 0; i < playersCount; i++)
            {
                _settings.PlayersIDs[i] = SecurityHandler.GetEncodedValue(_settings.PlayersKeys[i]);
            }
            _settings.PlayersDecks = data.PlayersDecks;
            _settings.PlayersMmrs = data.PlayersMmrs;
        }

        //private void ConvertRealmFixedKey()
        //{
        //    _settings.FixedRealmKey = System.Text.Encoding.UTF8.GetBytes(_fixedRealmKeyString);
        //}

        //private void GenerateRealmSessionKey()
        //{
        //    byte[] rsk = new byte[1024];
        //    _r.NextBytes(rsk);
        //    _settings.RealmSessionKey = rsk;
        //}


        public class RealmSettings
        {
            public int GardenPort;
            public int RealmPort;
            public long RealmID;
            public byte[] GardenKey;
            //public byte[] FixedRealmKey;
            // public string GameMode;

            public long[] PlayersIDs;
            public byte[][] PlayersKeys;
            public int[][] PlayersDecks;
            public int[] PlayersMmrs;
        }
    }

}
