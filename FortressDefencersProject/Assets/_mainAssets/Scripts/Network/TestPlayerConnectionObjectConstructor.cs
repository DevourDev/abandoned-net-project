using DevourDev.Base.SystemExtentions;
using DevourDev.Networking;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FD.Global
{
    public class TestPlayerConnectionObjectConstructor : IDisposable
    {
        private Socket _responsingSocket;
        private Socket _messagingSocket;

        private Task _countingDown;
        private CancellationTokenSource _cts;
        private Action<TestPlayerConnectionObjectConstructor, string> _onErrorAction;
        private Action<TestPlayerConnectionObjectConstructor> _objectReadyAction;

        // private readonly IPAddress _ip;
        private readonly long _accID;
        private readonly int _lifetime;

        private byte[] _key;


        public TestPlayerConnectionObjectConstructor(long accID, byte[] key, Action<TestPlayerConnectionObjectConstructor, string> onErrorAction,
            Action<TestPlayerConnectionObjectConstructor> objectAction, int lifetime)
        {
            _accID = accID;
            _key = key;
            _onErrorAction = onErrorAction;
            _objectReadyAction = objectAction;
            _lifetime = lifetime;
        }


        public long AccID => _accID;

        public Socket ResponsingSocket { get => _responsingSocket; private set => _responsingSocket = value; }
        public Socket MessagingSocket { get => _messagingSocket; private set => _messagingSocket = value; }

        private bool Ready => _responsingSocket != null && _messagingSocket != null;


        private void TriggerCountDown()
        {
            if (_countingDown != null)
                _cts.Cancel();

            if (Ready)
            {
                _objectReadyAction?.Invoke(this);
            }
            else
            {
                _cts = new();
                _countingDown = Task.Run(StartCountDown, _cts.Token);
            }
        }

        private async void StartCountDown()
        {
            try
            {
                await Task.Delay(_lifetime, _cts.Token);
                _cts.Token.ThrowIfCancellationRequested();
                _onErrorAction?.Invoke(this, $"timeout. lifetime: {_lifetime}");
            }
            catch (TaskCanceledException)
            {
            }
        }

        public bool CompareKeys(byte[] otherKey)
        {
            return _key.ArrayEqual(otherKey);
        }

        public void SetResponsingHandler(Socket s)
        {
            _responsingSocket = s;
            TriggerCountDown();
        }
        public void SetMessagingHandler(Socket s)
        {
            _messagingSocket = s;
            TriggerCountDown();
        }

        public void Dispose()
        {
            DevNet.CloseSocket(_messagingSocket);
            DevNet.CloseSocket(_responsingSocket);
        }
    }


}

