using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace FD.Networking.Garden.Packets
{
    public class EstablishRealmGardenConnectionRequest : IPacketContent
    {
        public int UniqueID => 50;
        public byte[] GardenKey;
        public byte[] FixedRealmKey;
        public byte[] RealmSessionKey;
        public int RealmPort;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            GardenKey = d.ReadBytes();
            FixedRealmKey = d.ReadBytes();
            RealmSessionKey = d.ReadBytes();
            RealmPort = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(GardenKey);
            e.Write(FixedRealmKey);
            e.Write(RealmSessionKey);
            e.Write(RealmPort);
        }
    }

    public class EstablishRealmGardenConnectionResponse : IPacketContent
    {
        public int UniqueID => 51;
        public bool Result;
        public byte[][] PlayersGardenKeys;
        public Error FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (Result)
                PlayersGardenKeys = d.ReadByteArrays();
            else
                FailReason = (Error)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (Result)
                e.Write(PlayersGardenKeys);
            else
                e.Write((int)FailReason);
        }

        public enum Error
        {
            PlayerLost,
            Other,
        }
    }
}
