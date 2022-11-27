
using DevourDev.Database.Interfaces;
using DevourEncoding.Serialization.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FD.Networking.Database.Entities.GameMatch
{
    public class GameMatch : IEntity
    {
        public long MatchID;
        public Team[] Teams;
        public int WinnerTeamIndex;
        public byte[] Replay;



        public long UniqueID { get => MatchID; set => MatchID = value; }

        public byte[] Encode()
        {
            return DevourXml.Serialize(this);
        }


        public class Team
        {
            public int TeamID;
            public long[] PlayersID;
        }
    }

}
