using System;

namespace FD.Networking.Realm
{
    /// <summary>
    /// Class for REALM-side to handle remote players
    /// </summary>
    public class RealmPlayerConnection : IDisposable
    {
        private readonly NetworkConnection _responsing;
        private readonly NetworkConnection _messaging;


        public RealmPlayerConnection(NetworkConnection responsing, NetworkConnection messaging)
        {
            _responsing = responsing;
            _messaging = messaging;
        }


        public NetworkConnection Responsing => _responsing;
        public NetworkConnection Messaging => _messaging;

        public void Dispose()
        {
            ((IDisposable)_responsing).Dispose();
            ((IDisposable)_messaging).Dispose();
        }
    }

}
