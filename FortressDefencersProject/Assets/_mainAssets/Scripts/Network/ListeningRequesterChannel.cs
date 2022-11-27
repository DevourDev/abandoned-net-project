using System;
using UnityEngine;

namespace FD.Networking
{
    public class ListeningRequesterChannel : IDisposable
    {
        public event Action<ListeningRequesterChannel> OnError;

        private readonly RequestingConnection _requestingConnection;
        private readonly ListeningConnection _listeningConnection;



        public ListeningRequesterChannel(RequestingConnection requestingConnection, ListeningConnection listeningConnection)
        {
            _requestingConnection = requestingConnection;
            _requestingConnection.Connection.OnError += HandleNetworkErrors;
            _listeningConnection = listeningConnection;
            _listeningConnection.Connection.OnError += HandleNetworkErrors;
        }



        public RequestingConnection RequestingConnection => _requestingConnection;
        public ListeningConnection ListeningConnection => _listeningConnection;

        public Exception LastException { get; private set; }

        public bool Errored => LastException != null;

        /// <summary>
        /// possibly
        /// </summary>
        public bool Connected
        {
            get
            {
                if (_requestingConnection == null)
                    return false;

                if (!_requestingConnection.Connection.Socket.Connected)
                    return false;

                if (_listeningConnection == null)
                    return false;

                if (!_listeningConnection.Connection.Socket.Connected)
                    return false;

                return true;
            }
        }


        private void HandleNetworkErrors(NetworkConnection c)
        {
            if (Errored)
                return;

            LastException = c.LastException;
            OnError?.Invoke(this);
        }



        public void Dispose()
        {
            try
            {
                ((IDisposable)_requestingConnection.Connection).Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogError("AAAAAAAAAAA: " + ex);
            }

            try
            {
                _listeningConnection.Connection.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogError("AAAAAAAAAAA: " + ex);
            }
        }
    }
}
