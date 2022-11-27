using FD.Networking.Database.Entities.Account;

namespace FD.Networking.Database.Packets
{
    public class GetAccountDataRequestSettings : DevourDev.Database.GetEntityRequestSettings<Account>
    {
        public bool GetPublicInfo;
        public bool GetLogInData;
        public bool GetSecureHistory;
        public bool GetGameStatistics;
        public bool IncludeGameHistory;
        public bool GetTemporary;
    }

    public class GetAccountDataResponseInfo
    {
        public bool HavePublicInfo;
        public bool HaveLogInData;
        public bool HaveSecureHistory;
        public bool HaveGameStatistics;
        public bool HaveTemporary;

        public GetAccountDataResponseInfo()
        {

        }

        public bool Result(GetAccountDataRequestSettings s)
        {
            switch (s.Mode)
            {
                case DevourDev.Database.Interfaces.GetEntityMode.AllOrNothing:
                    if (s.GetPublicInfo != HavePublicInfo)
                        return false;
                    if (s.GetLogInData != HaveLogInData)
                        return false;
                    if (s.GetSecureHistory != HaveSecureHistory)
                        return false;
                    if (s.GetGameStatistics != HaveGameStatistics)
                        return false;
                    if (s.GetTemporary != HaveTemporary)
                        return false;

                    return true;
                case DevourDev.Database.Interfaces.GetEntityMode.AllAvailable:
                    return HaveLogInData || HaveSecureHistory
                        || HaveGameStatistics || HaveTemporary;
                default:
                    return false;
            }


        }

    }
}
