using FD.Networking.Database.Entities.Account;
using FD.Networking.Database.Packets;
using FD.Networking.Packets;
using System;
using System.Threading.Tasks;

namespace FD.Networking.Garden
{
    public class GardenUserObject : IDisposable
    {
        public event Action<IPacketContent, GardenUserObject> OnRequestReceived;
        public event Action<GardenUserObject> OnError;

        private readonly ResponsingConnection _responsing;
        private readonly MessagingConnection _messaging;

        private readonly long _accID;
        private Account _account;
        private DateTime? _accountUpdatedDate;
        private bool _accountOutdated;
        private GetAccountDataRequestSettings _getAccSettings;
        private bool _errored;
        private bool _tmpBanned;
        private Lobby _foundGameLobby;
        private Lobby _realmLobby;

        public GardenUserObject(ResponsingConnection responsing, MessagingConnection messaging, long id)
        {
            _errored = false;
            _responsing = responsing;
            _messaging = messaging;
            _accID = id;
            Init();
        }




        public ResponsingConnection Responsing => _responsing;
        public MessagingConnection Messaging => _messaging;
        public long AccID => _accID;
        public GetAccountDataRequestSettings GetAccountSettings => _getAccSettings;


        /// <summary>
        /// probably..?
        /// </summary>
        public bool AccountIsActual
        {
            get
            {
                if (_accountOutdated)
                    return false;

                if (Account == null)
                    return false;

                if (!_accountUpdatedDate.HasValue)
                    return false;

                if ((DateTime.Now - _accountUpdatedDate)?.TotalMinutes > 10)
                    return false;

                return true;
            }

            set
            {
                _accountOutdated = !value;
            }
        }
        public Account Account => _account;
        public DateTime LastHeartBeat { get; private set; }
        public bool InFindGameQ { get; set; }
        public DateTime? FoundGameDate
        {
            get
            {
                if (_foundGameLobby != null)
                {
                    return _foundGameLobby.CreationDate;
                }

                return null;
            }
        }
        public bool InFoundGameLobby => _foundGameLobby != null;
        public bool InRealmGameLobby => _realmLobby != null;
        public bool AcceptedFoundGame
        {
            get
            {
                if (!InFoundGameLobby)
                    return false;

                if (!_foundGameLobby.Players.TryGetValue(AccID, out var ready))
                    return false;

                return ready;
            }

        }
        public bool TempBanned
        {
            get
            {
                if (!_tmpBanned)
                    return false;

                if (DateTime.Now > TempBanEndDate)
                {
                    _tmpBanned = false;
                    return false;
                }

                return true;
            }
        }
        public DateTime TempBanEndDate { get; private set; }

        public Lobby FoundGameLobby { get => _foundGameLobby; set => _foundGameLobby = value; }
        public Lobby RealmLobby { get => _realmLobby; set => _realmLobby = value; }


        public void Response(IPacketContent c) => Responsing.Connection.Response(c);
        /// <summary>
        /// rename me
        /// </summary>
        public bool TryGetActualAccount(GetAccountDataRequestSettings preferences, out Account acc)
        {
            acc = null;

            if (!AccountIsActual)
                return false;

            if (_getAccSettings.Mode != DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing)
                return false;

            if (preferences.GetPublicInfo != _getAccSettings.GetPublicInfo)
                return false;

            if (preferences.GetGameStatistics != _getAccSettings.GetGameStatistics)
                return false;

            if (preferences.GetLogInData != _getAccSettings.GetLogInData)
                return false;

            if (preferences.GetSecureHistory != _getAccSettings.GetSecureHistory)
                return false;

            if (preferences.GetTemporary != _getAccSettings.GetTemporary)
                return false;

            if (preferences.IncludeGameHistory != _getAccSettings.IncludeGameHistory)
                return false;

            acc = _account;
            return true;

        }
        public void AddTempBanTimeDuration(TimeSpan span)
        {
            _tmpBanned = true;
            TempBanEndDate += span;
        }

        public void CheckConnectionState()
        {
            Task.Run(() =>
            {
                var hbr = new HeartBeatRequest();
                _messaging.Message(hbr);
            });
        }

        private void Init()
        {
            LastHeartBeat = DateTime.Now;
            InFindGameQ = false;
            _responsing.OnError += Responsing_OnError;
            _messaging.OnError += Messaging_OnError;
            _responsing.OnRequestReceived += Responsing_OnRequestReceived;
        }

        private void Messaging_OnError(MessagingConnection obj)
        {
            if (!_errored)
            {
                _errored = true;
                OnError?.Invoke(this);
            }
        }

        private void Responsing_OnError(ResponsingConnection obj)
        {
            if (!_errored)
            {
                _errored = true;
                OnError?.Invoke(this);
            }
        }

        private void Responsing_OnRequestReceived(IPacketContent p, ResponsingConnection c)
        {
            switch (p)
            {
                case HeartBeatResponse:
                    LastHeartBeat = DateTime.Now;
                    break;
                default:
                    OnRequestReceived?.Invoke(p, this);
                    break;
            }
        }

        public void StartHandling()
        {
            _responsing.StartHandling();
        }


        public void Dispose()
        {
            _responsing?.Connection?.Dispose();
            _messaging?.Connection?.Dispose();
        }

        public void SetAccount(Account acc, GetAccountDataRequestSettings settings)
        {
            _accountUpdatedDate = DateTime.Now;
            _account = acc;
            _accountOutdated = false;
            _getAccSettings = settings;
        }
    }
}
