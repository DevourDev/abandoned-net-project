using System;

namespace FD.Networking.Database.Entities.Account
{
    public abstract class AccountSecureAction
    {
        public DateTime Date { get; set; }
        public byte[] Ip { get; set; }
        public int Port { get; set; }

        public AccountSecureAction()
        {

        }

    }
}
