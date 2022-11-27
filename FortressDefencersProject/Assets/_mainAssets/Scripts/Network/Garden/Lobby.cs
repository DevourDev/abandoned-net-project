using DevourDev.Base;
using DevourDev.MonoExtentions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FD.Networking.Garden
{
    public class Lobby : IUniqueReadonlyLong
    {
        public event Action<Lobby> OnLobbyDestroyed;
        public event Action<Lobby> OnAllPlayersReady;
        public event Action<Lobby> OnTimedOut;
        public event Action<Lobby, long> OnPlayerLeftLobby;


        private readonly object _locker = new();
        private readonly long _id;
        private System.Threading.CancellationTokenSource _timeOutCheckingCts;

        private readonly DateTime _creationDate;
        /// <summary>
        /// long - accountID, bool - readyState
        /// </summary>
        private readonly Dictionary<long, bool> _players;

        private DateTime? _timeOutDate;
        private bool _destroyed = false;


        public Lobby(long id)
        {
            _id = id;
            _creationDate = DateTime.Now;
            _players = new();
        }

        public Lobby(long id, TimeSpan timeout) : this(id)
        {
            _timeOutDate = _creationDate + timeout;
        }

        public Lobby(long id, DateTime timeOutDate) : this(id)
        {
            _timeOutDate = timeOutDate;
        }


        public DateTime CreationDate => _creationDate;
        public Dictionary<long, bool> Players => _players;

        public bool TimedOut
        {
            get
            {
                if (_timeOutDate != null && _timeOutDate.HasValue)
                    return DateTime.Now < _timeOutDate.Value;

                return false;
            }
        }
        public DateTime? TimeOutDate { get => _timeOutDate; protected set => _timeOutDate = value; }

        public long UniqueID => _id;


        public bool TryAddPlayer(long id, bool readyState = false)
        {
            if (Players.ContainsKey(id))
                return false;

            Players.Add(id, readyState);
            return true;
        }
        public void LeftLobby(long id)
        {
            if (Players.ContainsKey(id))
            {
                Players.Remove(id);
                OnPlayerLeftLobby?.Invoke(this, id);
            }
            else
            {
                this.InvokeOnMainThread(() => UnityEngine.Debug.Log($"Player ID {id} is not existing in dictionary."));
            }
        }
        public void SetReady(long id)
        {
            if (Players.ContainsKey(id))
            {
                Players[id] = true;
                CheckAllReady();
            }
            else
            {
                this.InvokeOnMainThread(() => UnityEngine.Debug.Log($"Player ID {id} is not existing in dictionary."));
            }
        }
        public void SetNotReady(long id)
        {
            if (Players.ContainsKey(id))
            {
                Players[id] = false;
            }
            else
            {
                this.InvokeOnMainThread(() => UnityEngine.Debug.Log($"Player ID {id} is not existing in dictionary."));
            }
        }

        public void StopCheckingForTimeOut()
        {
            if (_timeOutCheckingCts != null)
            {
                _timeOutCheckingCts.Cancel();
            }
            else
            {
                this.InvokeOnMainThread(() => UnityEngine.Debug.Log($"StopCheckingForTimeOut: checking is not running."));
                return;
            }
        }
        public void StartCheckingForTimeOut()
        {
            if (_timeOutCheckingCts == null)
            {
                _timeOutCheckingCts = new();
            }
            else
            {
                this.InvokeOnMainThread(() => UnityEngine.Debug.Log($"StartCheckingForTimeOut: already checking."));
                return;
            }

            var token = _timeOutCheckingCts.Token;

            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        var now = DateTime.Now;
                        var span = _timeOutDate.Value - now;
                        if (span.Ticks < 10_000)
                        {
                            _timeOutCheckingCts = null;
                            OnTimedOut?.Invoke(this);
                            return;
                        }

                        Task.Delay(span).Wait();

                    }
                }
                catch (OperationCanceledException)
                {
                    _timeOutCheckingCts = null;
                    return;
                }
                catch (Exception)
                {
                    return;
                }

            }, token);
        }

        private void CheckAllReady()
        {
            if (TimedOut)
            {
                OnTimedOut?.Invoke(this);
                return;
            }

            foreach (var p in Players)
            {
                if (!p.Value)
                    return;
            }

            OnAllPlayersReady?.Invoke(this);
        }

        public void AddTimeToTimeOut(TimeSpan extraTime)
        {
            if (TimeOutDate.HasValue)
            {
                TimeOutDate += extraTime;
            }
            else
            {
                TimeOutDate = DateTime.Now + extraTime;
            }


        }
        public void IterateAllPlayers(Action<long, bool> a, out Exception exc)
        {

            lock (_locker)
            {
                try
                {
                    foreach (var p in _players)
                    {
                        a?.Invoke(p.Key, p.Value);
                    }
                }
                catch (Exception ex)
                {
                    exc = ex;
                    return;
                }

                exc = null;
            }

        }

        public void Destroy()
        {
            if (_destroyed)
                return;

            _destroyed = true;
            OnLobbyDestroyed?.Invoke(this);
            _timeOutCheckingCts?.Cancel();
            _timeOutCheckingCts?.Dispose();

        }
    }
}
