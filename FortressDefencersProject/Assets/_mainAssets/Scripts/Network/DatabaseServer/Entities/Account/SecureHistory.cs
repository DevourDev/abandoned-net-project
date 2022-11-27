using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;

namespace FD.Networking.Database.Entities.Account
{
    public class SecureHistory
    {
        public List<AccountSecureAction> ActionsHistory;


        public SecureHistory()
        {
            ActionsHistory = new();
        }
        public SecureHistory(AccountSecureAction hi)
        {
            ActionsHistory = new();
            ActionsHistory.Add(hi);
        }

        public AccountSecureAction Registrated => ActionsHistory[0];
    }

    //public abstract class AccountAction
    //{
    //    public abstract DateTime Date { get; }
    //    public abstract IPAddress Ip { get; }
    //    public abstract int Port { get; }
    //}

    public class AccountCreatedAction : AccountSecureAction
    {
        public PlatformID PlatformID;
        public Version Version;

        public DateTime ClientDate;
        public string FirstClientSideHashedPassword;

        public AccountCreatedAction()
        {
            
        }
        public AccountCreatedAction(IPEndPoint ep, PlatformID platformID, Version version, DateTime clientDate, string firstClientSideHashedPassword)
        {
            Date = DateTime.Now;
            Ip = ep.Address.GetAddressBytes();
            Port = ep.Port;
            PlatformID = platformID;
            Version = version;
            ClientDate = clientDate;
            FirstClientSideHashedPassword = firstClientSideHashedPassword;
        }

    }
}
