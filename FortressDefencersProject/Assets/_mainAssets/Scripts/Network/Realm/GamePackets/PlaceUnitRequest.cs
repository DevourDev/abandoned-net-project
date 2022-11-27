using System;
using UnityEngine;

namespace FD.Networking.Realm.GamePackets
{
    public class PlaceUnitRequest : IPacketContent
    {
        public int UniqueID => 1010;
        public Vector3 SpawnPosition { get; set; }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SpawnPosition = d.ReadVector3();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SpawnPosition);
        }
    }

    public class PlaceUnitResponse : IPacketContent
    {
        public int UniqueID => 1011;
        public bool Result;
        public FailReasons FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (!Result)
                FailReason = (FailReasons)d.ReadByte();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (!Result)
                e.Write((byte)FailReason);
        }

        public enum FailReasons : byte
        {
            None,
            AreaIsBusy,
            NothingToPlace,
            PlayerLost,
        }
    }
}
