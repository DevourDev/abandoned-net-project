using DevourDev.Networking;

namespace FD.Networking.Realm.Packets
{
    public class EnterRealmRequest : IPacketContent
    {
        public int UniqueID => 10;
        public byte[] GardenSessionKey;
        public byte[] RealmKey;
        public ConnectionType ConnectionType;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            GardenSessionKey = d.ReadBytes();
            RealmKey = d.ReadBytes();
            ConnectionType = (ConnectionType)d.ReadInt();   

        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(GardenSessionKey);
            e.Write(RealmKey);
            e.Write((int)ConnectionType);
        }
    }

    public class EnterRealmResponse : IPacketContent
    {
        public int UniqueID => 11;
        public bool Result;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }
}
