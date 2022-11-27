namespace FD.Networking.Realm.GamePackets
{
    public class ConnectToRealmResponse : IPacketContent
    {
        public int UniqueID => 511;
        public bool Result;
        public Error FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (!Result)
                FailReason = (Error)d.ReadInt();

        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (!Result)
                e.Write((int)FailReason);
        }

        public enum Error
        {
            WrongSessionKey,
            InvalidConnectionTypeRequest,
            Timeout,
            Other
        }
    }


}
