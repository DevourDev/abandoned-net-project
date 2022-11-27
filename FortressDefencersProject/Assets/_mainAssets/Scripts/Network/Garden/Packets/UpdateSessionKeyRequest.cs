namespace FD.Networking.Garden.Packets
{
    public class UpdateSessionKeyRequest : IPacketContent
    {
        public int UniqueID => 100_010;
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

    public class UpdateSessionKeyResponse : IPacketContent
    {
        public int UniqueID => 100_011;
        public bool Result;
        public byte[] NewSessionKey;
        public Error FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (Result)
                NewSessionKey = d.ReadBytes();
            else
                FailReason = (Error)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            e.Write(NewSessionKey);
        }

        public enum Error
        {
            SessionKeyOutdated,
            Other,
        }
    }
}
