namespace FD.Networking.Database.Packets
{
    public class SetAccountDataRequest : IPacketContent
    {
        public int UniqueID => 20;
        public AccountSearchMode SearchMode;
        public long AccountID;
        public string AccountLogin;
        public SetAccountRequestSettings Settings;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SearchMode = (AccountSearchMode)d.ReadInt();
            switch (SearchMode)
            {
                case AccountSearchMode.ByID:
                    AccountID = d.ReadInt64();
                    break;
                case AccountSearchMode.ByLogin:
                    AccountLogin = d.ReadString();
                    break;
                default:
                    break;
            }
            Settings = d.ReadXml<SetAccountRequestSettings>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write((int)SearchMode);
            switch (SearchMode)
            {
                case AccountSearchMode.ByID:
                    e.Write(AccountID);
                    break;
                case AccountSearchMode.ByLogin:
                    e.Write(AccountLogin);
                    break;
                default:
                    break;
            }
            e.WriteXml(Settings);
        }

    }

    public class SetAccountDataResponse : IPacketContent
    {
        public int UniqueID => 21;
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
            AccountNotFound,
            Other,
        }
    }
}
