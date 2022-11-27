using DevourDev.MonoBase;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FD.Networking.Packets;

namespace FD.Networking
{
    public class TmpResponsingConnection : IDisposable
    {
        private readonly NetworkConnection _connection;
        private readonly int _lifetime;
        private readonly Action<TmpResponsingConnection, string> _failureAction;
        private readonly Action<IPacketContent, TmpResponsingConnection> _packetReceivedAction;
        private readonly FD_PacketsResolverBase _resolver;

        private readonly CancellationTokenSource _cts;

        public TmpResponsingConnection(Socket handler, int bufferSize, int lifetime,
             Action<TmpResponsingConnection, string> failureAcction, Action<IPacketContent, TmpResponsingConnection> dataReceivedAction, FD_PacketsResolverBase resolver)
        {
            _connection = new NetworkConnection(handler, bufferSize);
            _lifetime = lifetime;
            _failureAction = failureAcction;
            _packetReceivedAction = dataReceivedAction;
            _resolver = resolver;
            _cts = new();
        }


        public NetworkConnection Connection => _connection;

        public void Dispose()
        {
            _cts.Cancel();
            ((IDisposable)_connection).Dispose();
            ((IDisposable)_cts).Dispose();

        }

        public void Handle()
        {
            try
            {
                Task.Run(() =>
                {
                    Memory<byte> received = new();
                    _connection.OnDataReceivedAction = (data) => received = data;
                    Task countingDown = Task.Delay(_lifetime);
                    Task gettingPacket = Task.Run(() =>
                    {
                        _connection.ReceivePacket();
                    });

                    var first = Task.WaitAny(countingDown, gettingPacket);

                    if (first == 0)
                    {
                        _failureAction?.Invoke(this, $"timeout (lifetime was: {_lifetime} ms)");
                    }

                    else
                        _packetReceivedAction?.Invoke(Packet.Unpack(received, _resolver).Content, this);
                }, _cts.Token);

                //var selfdestruction = DevNet.ExecuteDelayed(_lifetime, _timeoutAction);
            }
            catch (SocketException sockEx)
            {
                //UnityEngine.Debug.LogError($"Socket error in TmpResponsingConnection.cs: {sockEx}");
                _failureAction?.Invoke(this, $"sockEx: {sockEx.Message}");
            }
            catch (TaskCanceledException tce)
            {
                _failureAction?.Invoke(this, $"TaskCanceledException: {tce.Message}");
            }
            catch (Exception ex)
            {
               // UnityEngine.Debug.LogError($"Handling task closed with error: " + ex.Message);
                _failureAction?.Invoke(this, ex.Message);
            }

        }


    }


}
