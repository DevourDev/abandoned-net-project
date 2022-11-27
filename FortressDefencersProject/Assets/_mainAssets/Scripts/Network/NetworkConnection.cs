using DevourDev.Networking;
using FD.Networking.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FD.Networking
{
    public class NetworkConnection : NetworkConnectionBase<NetworkConnection, FD_PacketsResolverBase, IPacketContent, Encoder, Decoder, Packet>
    {
        public event Action<NetworkConnection> OnError;


        public NetworkConnection(int bufferSize) : base(bufferSize)
        {
            OnErrorAction = HandleError;
        }

        public NetworkConnection(Socket connectedSocket, int bufferSize) : base(connectedSocket, bufferSize)
        {
            OnErrorAction = HandleError;
        }

        public NetworkConnection(EndPoint ep, int bufferSize) : base(ep, bufferSize)
        {
            OnErrorAction = HandleError;
        }


        public Exception LastException { get; private set; }

        public bool Errored => LastException != null;


        private void HandleError(object sender, Exception ex)
        {
            if (Errored)
                return;

            LastException = ex;
            OnError?.Invoke(this);
        }
    }

}
