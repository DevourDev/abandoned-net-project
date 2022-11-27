using System;
using System.Net.Sockets;
using FD.Networking.Packets;

namespace FD.Networking
{
    /// <summary>
    /// (Receives requests and sends responses to them)
    /// </summary>
    public class ResponsingConnection : IDisposable
    {
        public event Action<IPacketContent, ResponsingConnection> OnRequestReceived;
        public event Action<ResponsingConnection> OnError;

        private readonly NetworkConnection _connection;
        private readonly FD_PacketsResolverBase _resolver;


        public ResponsingConnection(NetworkConnection netCon, FD_PacketsResolverBase resolver)
        {
            _connection = netCon;
            _resolver = resolver;
            Init();
        }
        public ResponsingConnection(Socket handler, int bufferSize, FD_PacketsResolverBase resolver)
        {
            _connection = new(handler, bufferSize);
            _resolver = resolver;
            Init();
        }


        public NetworkConnection Connection => _connection;


        private void Init()
        {
            _connection.OnDataReceivedAction = (d) => OnRequestReceived?.Invoke(Packet.Unpack(d, _resolver).Content, this);
            _connection.OnError += (netCon) => OnError?.Invoke(this);
        }
        public void StartHandling()
        {
            _connection.StartReceivingLoop();
        }

        public void Dispose()
        {
            ((IDisposable)_connection).Dispose();
        }
    }
}