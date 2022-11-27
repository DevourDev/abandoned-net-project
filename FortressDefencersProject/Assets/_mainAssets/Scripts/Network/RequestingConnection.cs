using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using FD.Networking.Packets;

namespace FD.Networking
{
    /// <summary>
    /// (Sends requests and receives responses to them)
    /// </summary>
    public class RequestingConnection
    {
        public event Action<IPacketContent, RequestingConnection> OnResponseReceived;

        private readonly NetworkConnection _connection;
        private readonly FD_PacketsResolverBase _resolver;

        private readonly bool _locking;
        private readonly object _lockObject = new();


        public RequestingConnection(NetworkConnection connection, FD_PacketsResolverBase resolver, bool locking = false)
        {
            _connection = connection;
            _resolver = resolver;
            _locking = locking;
            Init();
        }

        public RequestingConnection(Socket connectedSocket, int bufferSize, FD_PacketsResolverBase resolver, bool locking = false)
        {
            _connection = new(connectedSocket, bufferSize);
            _resolver = resolver;
            _locking = locking;
            Init();
        }


        public NetworkConnection Connection => _connection;


        private void Init()
        {
            _connection.OnDataReceivedAction = (d) => OnResponseReceived?.Invoke(Packet.Unpack(d, _resolver).Content, this);
        }

        public IPacketContent Request(IPacketContent requestContent)
        {
            if (_locking)
            {
                lock (_lockObject)
                {
                    return _connection.Request(requestContent, _resolver);
                }
            }
            else
            {
                return _connection.Request(requestContent, _resolver);
            }
        }

        /// <summary>
        /// Returns nothing. Responses should be handled with callbacks.
        /// </summary>
        /// <param name="reqContent"></param>
        /// <returns></returns>
        public async Task SendRequestAsync(IPacketContent reqContent)
        {
            await _connection.SendDataAsync(Packet.CreateRequest(reqContent).ToBinaryMemory());
        }
        public async Task<IPacketContent> RequestAsync(IPacketContent requestContent)
            => await _connection.RequestAsync(requestContent, _resolver);

        public async Task<IPacketContent> RequestAsyncLocking(IPacketContent c)
            => await _connection.RequestAsyncLocking(c, _resolver);
    }
}