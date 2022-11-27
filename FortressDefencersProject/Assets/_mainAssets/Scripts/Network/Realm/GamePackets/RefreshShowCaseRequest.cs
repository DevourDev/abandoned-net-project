namespace FD.Networking.Realm.GamePackets
{
    public class RefreshShowCaseRequest : IPacketContent
    {
        public int UniqueID => 1020;


        public RefreshShowCaseRequest()
        {
        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }

    public class RefreshShowCaseResponse : IPacketContent
    {
        public int UniqueID => 1021;
        public bool Result;
        public FailureReason FailReason;


        public RefreshShowCaseResponse()
        {
        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            Result = d.ReadBool();
            if (!Result)
                FailReason = (FailureReason)d.ReadByte();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            e.Write((byte)FailReason);
        }

        public enum FailureReason : byte
        {
            None,
            NotEnoughCoins,
            Blocked,
            Other,
        }
    }
}