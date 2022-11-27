using System;
using System.Net.Sockets;
using FD.Networking.Packets;

namespace FD.Networking
{
    /// <summary>
    /// (Listening to messages)
    /// </summary>
    public class ListeningConnection
    {
        public event Action<IPacketContent, ListeningConnection> OnMessageReceived;
        public event Action<ListeningConnection> OnError;

        private readonly NetworkConnection _connection;
        private readonly FD_PacketsResolverBase _resolver;

        public ListeningConnection(Socket handler, int bufferSize, FD_PacketsResolverBase resolver)
        {
            _connection = new(handler, bufferSize);
            _resolver = resolver;
            Init();
        }

        public ListeningConnection(NetworkConnection connection, FD_PacketsResolverBase resolver)
        {
            _connection = connection;
            _resolver = resolver;
            Init();
        }

        public NetworkConnection Connection => _connection;


        private void Init()
        {
            _connection.OnDataReceivedAction = (d) => OnMessageReceived?.Invoke(Packet.Unpack(d, _resolver).Content, this);
            _connection.OnError += (netCon) => OnError?.Invoke(this);
        }
        public void StartListening()
        {
            _connection.StartReceivingLoop();
        }
    }
}