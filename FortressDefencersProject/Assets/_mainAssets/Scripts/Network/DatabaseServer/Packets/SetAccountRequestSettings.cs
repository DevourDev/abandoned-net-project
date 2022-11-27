using FD.Networking.Database.Entities.Account;

namespace FD.Networking.Database.Packets
{
    public class SetAccountRequestSettings : DevourDev.Database.SetEntityRequestSettings<Account>
    {
        public bool SetPublicInfo;
        public bool SetLogInData;
        public bool SetSecureHistory;
        public bool SetLogInDataGameStatistics;
        public bool SetTemporary;
    }
}
