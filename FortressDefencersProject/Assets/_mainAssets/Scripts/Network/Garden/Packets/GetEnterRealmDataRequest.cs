namespace FD.Networking.Garden.Packets
{

    public class GetEnterRealmDataRequest : IPacketContent
    {
        public int UniqueID => 40;
        public byte[] SessionKey;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SessionKey = d.ReadBytes();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SessionKey);
        }
    }

    public class GetEnterRealmDataResponse : IPacketContent
    {
        public int UniqueID => 41;
        public bool Result;
        public byte[] RealmKey;
        public Error FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (Result)
                RealmKey = d.ReadBytes();
            else
                FailReason = (Error)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (Result)
                e.Write(RealmKey);
            else
                e.Write((int)FailReason);
        }

        public enum Error
        {
            WrongSessionKey,
            TimeOut,
            Ban,
            Other,
        }
    }
}
