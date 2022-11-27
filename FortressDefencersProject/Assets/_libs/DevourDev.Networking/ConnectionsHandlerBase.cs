using DevourDev.MonoExtentions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DevourDev.Networking
{

    public class ConnectionsHandlerBase : IDisposable
    {
        public event Action<Socket> OnNewConnection;

        private readonly Socket _mainSocket;
        private readonly CancellationTokenSource _acceptingCancellation;
        private readonly int _maxConnectionsQ;


        public ConnectionsHandlerBase(int port, int maxConnectionsQ)
        {
            _maxConnectionsQ = maxConnectionsQ;
            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _mainSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _acceptingCancellation = new();
        }


        public bool Accepting { get; set; }
        public Socket MainSocket => _mainSocket;



        public void StartAccepting()
        {
            Accepting = true;

            _mainSocket.Listen(_maxConnectionsQ);

            Task.Factory.StartNew(AcceptingLoop, _acceptingCancellation.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void CancelAccepting()
        {
            _acceptingCancellation.Cancel();
            DevNet.CloseSocket(_mainSocket);
        }

        private void AcceptingLoop()
        {
            try
            {
                while (true)
                {
                    _acceptingCancellation.Token.ThrowIfCancellationRequested();
                    this.InvokeOnMainThread(() =>
                     {
                         UnityEngine.Debug.Log("Accepting Loop started.");

                     });
                    try
                    {
                        var handler = _mainSocket.Accept();
                        OnNewConnection?.Invoke(handler);
                    }
                    catch (SocketException sockEx)
                    {
                        UnityEngine.Debug.LogWarning("SocketException in Accepting Loop: " + sockEx.Message);
                    }

                }
            }
            catch (OperationCanceledException cancelEx)
            {
                UnityEngine.Debug.Log("Accepting Loop cancelled: " + cancelEx.Message);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("Accepting Loop cancelled with error: " + ex.Message);
            }

        }


        public void Dispose()
        {
            if (_acceptingCancellation != null)
                _acceptingCancellation.Cancel();
            if (_mainSocket != null)
                ((IDisposable)_mainSocket).Dispose();
            if (_acceptingCancellation != null)
                ((IDisposable)_acceptingCancellation).Dispose();
        }
    }


}
