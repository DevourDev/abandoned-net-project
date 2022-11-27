using DevourDev.Base;
using FD.Networking.Realm;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FD.Networking.Garden
{
    /// <summary>
    /// GARDEN-SIDE
    /// </summary>
    public class GardenRealmConnection : IDisposable, IUniqueLong
    {
        public enum RealmState : byte
        {
            None,
            Initializing,
            GameInProgress,
            Available,
            Failed,
        }

        public event Action<IPacketContent, GardenRealmConnection> OnRequestReceived;

        private const int BUFFER_SIZE = 10_240;

        private ResponsingConnection _connection;
        private GardenRealmPacketsResolver _resolver;
        private byte[] _gardenKey;

        private RealmState _state;
        private byte[][] _playersKeys = null;
        private int[][] _playersDecks = null;
        private int[] _playersMmrs = null;
        private System.Net.IPEndPoint _realmIpepForPlayers = null;
        private System.Threading.ManualResetEvent _waitingInitRequestMre;

        private bool _disposedValue;


        public GardenRealmConnection(byte[] gardenKey, GardenRealmPacketsResolver resolver)
        {
            _gardenKey = gardenKey;
            _resolver = resolver;
            _state = RealmState.None;
        }


        public long UniqueID { get; set; }
        public Lobby Lobby { get; set; }
        public ResponsingConnection Responsing => _connection ?? null;
        public IPEndPoint RealmIpepForPlayers => _realmIpepForPlayers;
        public RealmState State => _state;


        public void SetPlayersKeys(byte[][] keys) => _playersKeys = keys;

        public void Response(IPacketContent c) => Responsing.Connection.Response(c);

        public bool CompareKeys(byte[] other)
        {
            return _gardenKey.SequenceEqual(other);
        }

        public bool InitConnection(Socket s)
        {
            try
            {
                var c = new NetworkConnection(s, BUFFER_SIZE);
                _connection = new(c, _resolver);
                _connection.OnRequestReceived += Connection_OnRequestReceived;
            }
            catch (Exception)
            {
                _state = RealmState.Failed;
                _connection = null;
                return false;
            }

            _state = RealmState.Available;
            return true;
        }

        System.Diagnostics.Stopwatch _sw = new();
        public async Task StartHandling(byte[][] playerKeys, int[][] playerDecks, int[] playersMmrs)
        {
            _playersKeys = playerKeys;
            _playersDecks = playerDecks;
            _playersMmrs = playersMmrs;

            _waitingInitRequestMre = new(false);
            _connection.StartHandling();

            DevourDev.MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                UnityEngine.Debug.Log($"GardenRealmConnection.StartHandling before waiting.");
            });
            _sw.Restart();
            await Task.Run(() =>
            {
                _waitingInitRequestMre.WaitOne();
            });
            _sw.Stop();

            DevourDev.MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                UnityEngine.Debug.Log($"GardenRealmConnection.StartHandling awaiting time: {_sw.Elapsed.TotalMilliseconds} ms");
            });


        }

        public bool CheckPlayersKeys(out int keysCount)
        {
            if (_playersKeys == null)
            {
                keysCount = -1;
                return false;
            }

            keysCount = _playersKeys.Length;

            for (int i = 0; i < keysCount; i++)
            {
                if (_playersKeys[i] == null)
                    return false;
            }

            return true;
        }
        private void Connection_OnRequestReceived(IPacketContent p, ResponsingConnection con)
        {
            switch (p)
            {
                case UsersInitialDataRequest uidreq:
                    HandleUsersInitialDataRequest(uidreq, con);
                    break;
                default:
                    OnRequestReceived?.Invoke(p, this);
                    break;
            }
        }

        private void HandleUsersInitialDataRequest(UsersInitialDataRequest uidreq, ResponsingConnection con)
        {
            //todo: let Realm know his Outter IP and send not only port, but full EndPoint
            var ep = new IPEndPoint(IPAddress.Parse("85.30.248.243"), uidreq.RealmPortForPlayers);
            //var ep = new IPEndPoint(IPAddress.Loopback, uidreq.RealmPortForPlayers);
            _realmIpepForPlayers = ep;

            if (_playersKeys == null)
                throw new NullReferenceException($"{nameof(_playersKeys)} is null.");

            if (_playersDecks == null)
                throw new NullReferenceException($"{nameof(_playersDecks)} is null.");

            if (_playersMmrs == null)
                throw new NullReferenceException($"{nameof(_playersMmrs)} is null.");

            for (int i = 0; i < _playersKeys.Length; i++)
            {
                if (_playersKeys[i] == null)
                    throw new NullReferenceException($"{nameof(_playersKeys)}[{i}] is null.");
            }

            UsersInitialDataResponse res = new();
            //res.PlayersDecks = new int[2][] { new int[1] { 0 }, new int[1] { 0 } };
            //int[] p1Deck = { 0 };
            //int[] p2Deck = { 0 };
            //int[][] playersDecks = new int[2][] { p1Deck, p2Deck };

            res.PlayersDecks = _playersDecks;
            res.PlayersEnterRealmKeys = _playersKeys;
            res.PlayersMmrs = _playersMmrs;

            res.Result = true;

            con.Connection.Response(res);

            _waitingInitRequestMre.Set();
        }


        #region disposing
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                _connection.Dispose();
                Lobby.Destroy();
                // TODO: установить значение NULL для больших полей
                _gardenKey = null;
                Lobby = null;
                _connection = null;
                _resolver = null;
                _disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~GardenRealmConnection()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }


}
