using FD.Networking;
using System;
using System.Collections.Generic;

namespace FD.Global
{
    public class NetworkPlayerObject
    {
        public event Action<NetworkPlayerObject> OnError;

        private readonly ResponsingConnection _responsing;
        private readonly MessagingConnection _messaging;

        private readonly long _accID;



        public NetworkPlayerObject(ResponsingConnection responsing, MessagingConnection messaging, long id)
        {
            _responsing = responsing;
            _messaging = messaging;
            _accID = id;
           
            Init();
        }


        public ResponsingConnection Responsing => _responsing;
        public MessagingConnection Messaging => _messaging;
        public long AccID => _accID;







        private void Init()
        {
            _responsing.OnError += (rc) => OnError?.Invoke(this);
            _messaging.OnError += (mc) => OnError?.Invoke(this);
        }


        public void StartHandling()
        {
            _responsing.StartHandling();
        }


        public void Dispose()
        {
            _responsing.OnError -= (rc) => OnError?.Invoke(this);
            _messaging.OnError -= (mc) => OnError?.Invoke(this);
            try
            {
                _responsing.Connection.Dispose();
            }
            catch (Exception)
            {

            }
            try
            {
                _messaging.Connection.Dispose();
            }
            catch (Exception)
            {

            }
           
           
        }
    }
}