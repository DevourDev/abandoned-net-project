using FD.Networking.Database.Entities.Account;

namespace FD.Networking.Database.Packets
{
    public class GetAccountDataRequest : IPacketContent
    {
        public int UniqueID => 10;
        public AccountSearchMode SearchMode;
        public long AccountID;
        public string AccountLogin;
        public GetAccountDataRequestSettings Settings;

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
            Settings = d.ReadXml<GetAccountDataRequestSettings>();
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

        public static GetAccountDataRequest GetTmpData(long accID)
        {
            return new GetAccountDataRequest
            {
                AccountID = accID,
                SearchMode = AccountSearchMode.ByID,
                Settings = new GetAccountDataRequestSettings
                {
                    GetTemporary = true,
                    Mode = DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing,
                },
            };
        }
        public static GetAccountDataRequest GetTmpData(string accLogin)
        {
            return new GetAccountDataRequest
            {
                AccountLogin = accLogin,
                SearchMode = AccountSearchMode.ByLogin,
                Settings = new GetAccountDataRequestSettings
                {
                    GetTemporary = true,
                    Mode = DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing,
                },
            };
        }
    }
    public class GetAccountDataResponse : IPacketContent
    {
        public int UniqueID => 11;
        public bool Result;
        public GetAccountDataResponseInfo Info;
        public Account AccountData;
        public Error FailReason;

        public GetAccountDataResponse()
        {
            Info = new();
            AccountData = new();
        }

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (Result)
            {
                Info = d.ReadXml<GetAccountDataResponseInfo>();
                AccountData = d.ReadXml<Account>();
            }
            else
            {
                FailReason = (Error)d.ReadInt();
            }
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (Result)
            {
                e.WriteXml(Info);
                e.WriteXml(AccountData);
            }
            else
            {
                e.Write((int)FailReason);
            }
        }

        public enum Error
        {
            AccountNotFound,
            RequestedEntitiesMissing,
            Other,
        }
    }
}
