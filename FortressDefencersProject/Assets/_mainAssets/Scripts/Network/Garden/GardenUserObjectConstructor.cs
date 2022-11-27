using DevourDev.Networking;
using DevourDev.MonoBase;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FD.Networking.Garden
{
    public class GardenUserObjectConstructor : IDisposable
    {

        private Socket _responsingSocket;
        private Socket _messagingSocket;

        private readonly byte[] _sessionKey;
        private readonly long _accID;
        private Action<GardenUserObjectConstructor, string> _onErrorAction;
        private Action<GardenUserObjectConstructor> _objectReadyAction;
        private readonly int _lifetime;
        private Task _countingDown;
        private System.Threading.CancellationTokenSource _cts;

        public GardenUserObjectConstructor(byte[] sessionKey, long accID,
            Action<GardenUserObjectConstructor, string> onErrorAction,
            Action<GardenUserObjectConstructor> objectReadyAction, int lifeTime)
        {
            _sessionKey = sessionKey;
            _accID = accID;
            _onErrorAction = onErrorAction;
            _objectReadyAction = objectReadyAction;
            _lifetime = lifeTime;
        }


        public long AccID => _accID;
        private bool Ready => _responsingSocket != null &&_responsingSocket.Connected && _messagingSocket != null && _messagingSocket.Connected;

        public Socket ResponsingSocket => _responsingSocket;
        public Socket MessagingSocket => _messagingSocket;


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
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {

                UnityEngine.Debug.Log("Countdown started!");
            });
            try
            {
                await Task.Delay(_lifetime, _cts.Token);
                _cts.Token.ThrowIfCancellationRequested();
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    UnityEngine.Debug.Log("Countdown ended (invoking _timeoutAction)");

                    UnityEngine.Debug.Log("Rejecting connection from GardenUserObjectConstructor.StartCountDown()...");
                });
                _onErrorAction?.Invoke(this, "timeout");
            }
            catch (TaskCanceledException)
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    UnityEngine.Debug.Log("Countdown canceled!");
                });
            }
            catch (OperationCanceledException)
            {
                UnityMainThreadDispatcher.InvokeOnMainThread(() =>
                {
                    UnityEngine.Debug.Log("Countdown canceled!");
                });
            }
            catch (Exception ex)
            {
                _onErrorAction?.Invoke(this, $"unexpected errpor: {ex.Message}");
            }
        }


        public bool CompareKeys(byte[] otherKey)
        {
            return _sessionKey.SequenceEqual(otherKey);
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
