namespace FD.Networking.Database.Entities.Account
{
    public class AccountStateData
    {
        public long ID;
        public LogInData LogInData;
        public TmpData TmpData;

        public AccountStateData()
        {

        }
        public AccountStateData(Account acc)
        {
            ID = acc.AccountID;
            LogInData = acc.SecureData.LogInData;
            TmpData = acc.Temporary;
        }
    }


}
