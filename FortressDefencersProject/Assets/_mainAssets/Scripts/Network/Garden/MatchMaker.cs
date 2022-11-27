using System;
using System.Collections.Generic;
using FD.Networking.Database.Entities.Account;
using DevourDev.Base.SystemExtentions;
using DevourDev.MonoExtentions;

namespace FD.Networking.Garden
{
    public class MatchMaker
    {
        public event Action<long[]> OnFinishTeam;

        private readonly List<PlayerInQueue> _queue;


        public MatchMaker()
        {
            _queue = new();
        }


        public int QueueCount => _queue.Count;


        public void RemovePlayerFromQueue(long accID)
        {
            for (int i = 0; i < _queue.Count; i++)
            {
                if (_queue[i].AccID == accID)
                {
                    _queue.RemoveAt(i);
                    goto PlayerRemoved;
                }
            }

            this.InvokeOnMainThread(() => UnityEngine.Debug.LogError($"User with ID {accID} not existing in matchmaking queue."));

            return;

        PlayerRemoved:
            SortQ();
        }
        public void AddPlayerToQueue(Account acc)
        {
            PlayerInQueue piq = new(acc);
            if (TryFindOpponent_prtp(piq, out var worthyOpponent))
            {
                _queue.Remove(worthyOpponent);
                this.InvokeOnMainThread(() => UnityEngine.Debug.LogError($"p1 id: {piq.AccID}, p2 id: {worthyOpponent.AccID}"));
                FinishTeam(piq.AccID, worthyOpponent.AccID);
            }
            else
            {
                _queue.Add(piq);
            }

            SortQ();
        }

        private void SortQ()
        {
            _queue.Sort((p1, p2) => p1.Strength.CompareTo(p2.Strength));
        }
        private bool TryFindOpponent_prtp(PlayerInQueue piq, out PlayerInQueue worthyOpponent)
        {
            if (_queue.Count == 0)
            {
                worthyOpponent = null;
                return false;
            }

            foreach (var q in _queue)
            {
                if(q.Strength - piq.Strength < piq.AllowedStrengthDifference)
                {
                    worthyOpponent = q;
                    return true;
                }
            }

            worthyOpponent = null;
            return false;


            var searchResult = _queue.SoftBinarySearch(piq);
            switch (searchResult.Type)
            {
                case ArrayExtentions.SoftBinarySearchResult.ResultType.Exact:
                    worthyOpponent = _queue[searchResult.Index];
                    return true;
                case ArrayExtentions.SoftBinarySearchResult.ResultType.Lower:
                    var weakerOpponent = _queue[searchResult.Index];
                    var strongerOpponent = _queue[searchResult.Index + 1];
                    var weakerOppDifference = Math.Abs(piq.Strength - weakerOpponent.Strength);
                    var strongerOppDifference = Math.Abs(piq.Strength - strongerOpponent.Strength);

                    if (weakerOppDifference > strongerOppDifference)
                        worthyOpponent = strongerOpponent;
                    else
                        worthyOpponent = weakerOpponent;
                    break;
                case ArrayExtentions.SoftBinarySearchResult.ResultType.Closest:
                    worthyOpponent = _queue[searchResult.Index];
                    break;
                default:
                    worthyOpponent = null;
                    return false;
            }

            return Math.Abs(worthyOpponent.Strength - piq.Strength)
                      <= Math.Min(piq.AllowedStrengthDifference, worthyOpponent.AllowedStrengthDifference);
        }

        private void FinishTeam(params long[] players)
        {
            OnFinishTeam?.Invoke(players);
        }

        private class PlayerInQueue : IComparable<PlayerInQueue>
        {
            public PlayerInQueue(Account acc)
            {
                AccID = acc.AccountID;
                CalculateStrength(acc);
            }


            public long AccID { get; private set; }
            public float Strength { get; private set; }
            public DateTime EnqueueDate { get; set; }
            public float AllowedStrengthDifference { get; set; } = 50;

            public int CompareTo(PlayerInQueue other)
            {
                return Strength.CompareTo(other.Strength);
            }

            private void CalculateStrength(Account acc)
            {
                Strength = 100f;
                //TODO: сила определяется на основе:
                //количества матчей,
                //винрейта за последние Х игр
                //либо рейтинга =-)
            }

        }
    }
}
