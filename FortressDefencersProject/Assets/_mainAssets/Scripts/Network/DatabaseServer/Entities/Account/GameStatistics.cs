using System.Collections.Generic;

namespace FD.Networking.Database.Entities.Account
{
    public class GameStatistics
    {
        /// <summary>
        /// Match IDs
        /// </summary>
        public List<long> GamesHistory;
        public int Wins;
        public int Total;
        public int Mmr;


        public GameStatistics()
        {
            GamesHistory = new();
            Wins = 0;
            Total = 0;
            Mmr = 0;
        }
    }
}