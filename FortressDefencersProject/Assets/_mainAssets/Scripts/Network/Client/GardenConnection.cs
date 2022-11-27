namespace FD.Networking.Client
{
    public class GardenConnection
    {
        private readonly ListeningRequesterChannel _channel;


        public GardenConnection(ListeningRequesterChannel channel)
        {
            _channel = channel;
        }


        public ListeningRequesterChannel Channel => _channel;

        public byte[] SessionKey { get; private set; }


        public void SetSessionKey(byte[] k) => SessionKey = k;

    }
}