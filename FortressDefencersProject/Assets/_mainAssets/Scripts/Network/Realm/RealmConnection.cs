using DevourDev.MonoExtentions;
using FD.Networking.Client;
using FD.Networking.Realm.GamePackets;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FD.Networking.Realm
{
    /// <summary>
    /// CLIENT-side
    /// </summary>
    public class RealmConnection : IDisposable
    {
        public event Action<IPacketContent> OnRealmResponseReceived;
        public event Action<IPacketContent> OnRealmMessageReceived;

        private ListeningRequesterChannel _channel;

        private byte[] _key;
        private System.Net.IPEndPoint _ipep;
        private FD_RealmGamePacketsResolver _realmGamePacketsResolver;
        private Action<RealmConnection> _onErrorAction;


        public RealmConnection()
        {
            _realmGamePacketsResolver = new();
        }


        public ListeningRequesterChannel Channel => _channel;

        public void SetKey(byte[] key)
        {
            _key = key;
        }

        public void SetIpep(System.Net.IPEndPoint ipep)
        {
            _ipep = ipep;
        }


        public async Task<bool> TryConnect(System.Action<RealmConnection> onErrorAction = null)
        {
            //check if already connected
            this.InvokeOnMainThread(() => Debug.LogError($"RealmPlayerConnection.TryConnect()."));
            NetworkConnection requestingNC = new(ClientManager.BUFFER_SIZE);
            NetworkConnection listeningNC = new(ClientManager.BUFFER_SIZE);

            await requestingNC.ConnectAsync(_ipep);
            this.InvokeOnMainThread(() => Debug.LogError($"RealmPlayerConnection: requestingNC.ConnectAsync awaited.")); //last log (now not last)
            ConnectToRealmRequest ctrAsRequesterRequest = new();
            ctrAsRequesterRequest.Key = _key;
            ctrAsRequesterRequest.ConnectionType = DevourDev.Networking.ConnectionType.Requester;
            var waitingForReqConResponse = requestingNC.RequestAsync(ctrAsRequesterRequest, _realmGamePacketsResolver);
            var reqConResponse = await waitingForReqConResponse;
            this.InvokeOnMainThread(() => Debug.LogError($"RealmPlayerConnection: requestingNC.RequestAsync awaited."));


            if (reqConResponse is not ConnectToRealmResponse reqRes)
            {
                this.InvokeOnMainThread(() => Debug.LogError("Requesting connection: received damaged request."));
                return false;
            }
            if (!reqRes.Result)
            {
                this.InvokeOnMainThread(() => Debug.LogError("Requesting connection request was denied."));
                return false;
            }

            await listeningNC.ConnectAsync(_ipep);
            this.InvokeOnMainThread(() => Debug.LogError($"RealmPlayerConnection: listeningNC.ConnectAsync awaited.")); // last log

            ConnectToRealmRequest ctrAsListenerRequest = new();
            ctrAsListenerRequest.Key = _key;
            ctrAsListenerRequest.ConnectionType = DevourDev.Networking.ConnectionType.MessagesListener;
            var waitingForLisConResponse = listeningNC.RequestAsync(ctrAsListenerRequest, _realmGamePacketsResolver);
            var lisConResponse = await waitingForLisConResponse;

            if (lisConResponse is not ConnectToRealmResponse lisRes)
            {
                this.InvokeOnMainThread(() => Debug.LogError("Listening connection: received damaged request."));
                return false;
            }
            if (!lisRes.Result)
            {
                this.InvokeOnMainThread(() => Debug.LogError("Listening connection request was denied."));
                return false;
            }

            this.InvokeOnMainThread(() => Debug.LogError("Successfully connected to Realm!"));

            var reqCon = new RequestingConnection(requestingNC, _realmGamePacketsResolver);
            var lisCon = new ListeningConnection(listeningNC, _realmGamePacketsResolver);

            _channel = new ListeningRequesterChannel(reqCon, lisCon);

            if (onErrorAction != null)
                _onErrorAction = onErrorAction;

            _channel.OnError += Channel_OnError;
            _channel.RequestingConnection.OnResponseReceived += RealmChannel_OnResponseReceived;
            _channel.ListeningConnection.OnMessageReceived += RealmChannel_OnMessageReceived;

            this.InvokeOnMainThread(() => Debug.LogError("Realm Connection inited."));

            _channel.RequestingConnection.Connection.StartReceivingLoop();
            _channel.ListeningConnection.StartListening();
            return true;
        }

        private void RealmChannel_OnMessageReceived(IPacketContent p, ListeningConnection arg2)
        {
            switch (p)
            {
                default:
                    OnRealmMessageReceived?.Invoke(p);
                    break;
            }
        }

        private void RealmChannel_OnResponseReceived(IPacketContent p, RequestingConnection arg2)
        {
            switch (p)
            {
                default:
                    OnRealmResponseReceived?.Invoke(p);
                    break;
            }
        }

        private void Channel_OnError(ListeningRequesterChannel obj)
        {
            this.InvokeOnMainThread(() => Debug.LogError($"Error in client's RealmChannel: {obj.LastException}"));
            _onErrorAction?.Invoke(this);
        }

        public void Dispose()
        {
            try
            {
                _channel.OnError -= Channel_OnError;
                if (_channel != null)
                    ((IDisposable)_channel).Dispose();
            }
            catch (Exception) { }


            _key = null;
        }
    }

}
