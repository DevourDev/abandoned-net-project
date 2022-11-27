using System.Collections.Generic;
using UnityEngine;

namespace FD.Networking.Realm.GamePackets
{
    public class FullGameStateResponse : IPacketContent
    {
        public int UniqueID => 521;
        public bool Result;
        public PlayerNewResourcesPacket[] PlayersResources;
        //add states and abilities and etc...

        private bool _filledWithLists;
        private List<PlayerNewResourcesPacket> _playersResources;

        public FullGameStateResponse()
        {

        }

        public FullGameStateResponse(List<PlayerNewResourcesPacket> playersResources)
        {
            _filledWithLists = true;
            _playersResources = playersResources;
        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();

            if (!Result)
                return;

            PlayersResources = d.ReadDecodables<PlayerNewResourcesPacket>();

        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);

            if (!Result)
                return;

            if (_filledWithLists)
            {
                e.WriteEncodables(_playersResources);
            }
            else
            {
                e.Write(PlayersResources);
            }
        }
    }


}
