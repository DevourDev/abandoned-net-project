using FD.Networking;
using System;
using System.Collections.Generic;

namespace FD.Global.Sides
{
    public class GameSideOnline : GameSideDefault
    {
        private NetworkPlayerObject _networkPlayerObject;
        private ConnectionInfo _connectionInfo;


        public GameSideOnline() : base()
        {
        }


        public NetworkPlayerObject NetworkPlayerObject => _networkPlayerObject;
        public OnlineConnectionStatus ConnectionStatus => _connectionInfo.Status;

        public long AccID { get; private set; }
        public int Mmr { get; private set; }

        public int WinMmr { get; set; }
        public int LoseMmr { get; set; }

        public void Init(NetworkPlayerObject npo, long accID, int mmr)
        {
            if (_connectionInfo == null)
                _connectionInfo = new ConnectionInfo();
            else
                _connectionInfo.RegistrateConnection();

            _networkPlayerObject = npo;
            _networkPlayerObject.Responsing.OnRequestReceived += (req, con) => RequestsQ.Enqueue(req);
            _networkPlayerObject.OnError += HandleNetworkError;
            AccID = accID;
            Mmr = mmr;
        }

        private void HandleNetworkError(NetworkPlayerObject npo) //todo: do
        {
            npo.Dispose();
            _connectionInfo.RegistarateDisconnection();
        }

        public override void Dispose()
        {
            base.Dispose();
            _networkPlayerObject.Dispose();
        }
        protected override void HandleResponse(IPacketContent res)
        {
            if (ConnectionStatus != OnlineConnectionStatus.OK)
                return;

            _networkPlayerObject.Responsing.Connection.Response(res);
        }


        private class ConnectionInfo
        {
            private readonly List<DateTime> _connectionsDates;
            private readonly List<DateTime> _disconnectionsDates;

            private OnlineConnectionStatus _status;
            private DateTime? _leaveDate;

            public ConnectionInfo()
            {
                _connectionsDates = new();
                RegistrateConnection();
                _disconnectionsDates = new();
                _leaveDate = null;
            }

            public OnlineConnectionStatus Status => _status;

            public DateTime FirstConnectDate => _connectionsDates[0];
            public DateTime LastConnectDate => _connectionsDates[^1];
            public DateTime? FirstDisconnectDate => _disconnectionsDates.Count > 0 ? _disconnectionsDates[0] : null;
            public DateTime? LastDisconnectDate => _disconnectionsDates.Count > 0 ? _disconnectionsDates[^1] : null;


            public void RegistarateDisconnection()
            {
                _disconnectionsDates.Add(DateTime.Now);
                _status = OnlineConnectionStatus.Disconnected;
            }

            public void RegistrateConnection()
            {
                _connectionsDates.Add(DateTime.Now);
                _status = OnlineConnectionStatus.OK;

            }

            public void RegistrateLeave()
            {
                _leaveDate = DateTime.Now;
                _status = OnlineConnectionStatus.Left;
            }
        }
    }
}