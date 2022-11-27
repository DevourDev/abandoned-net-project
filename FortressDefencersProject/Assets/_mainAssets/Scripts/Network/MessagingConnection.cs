using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FD.Networking
{
    /// <summary>
    /// (Sends messages)
    /// </summary>
    public class MessagingConnection
    {
        public event Action<MessagingConnection> OnError;

        private readonly NetworkConnection _connection;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="bufferSize">not needed mostly</param>
        public MessagingConnection(Socket handler, int bufferSize = 512)
        {
            _connection = new(handler, bufferSize);
            Init();
        }


        public NetworkConnection Connection => _connection;


        private void Init()
        {
            _connection.OnError += (netCon) => OnError?.Invoke(this);
        }

        public void Message(IPacketContent message)
            => _connection.Message(message);

        public async Task MessageAsync(IPacketContent message)
            => await _connection.MessageAsync(message);
    }
}