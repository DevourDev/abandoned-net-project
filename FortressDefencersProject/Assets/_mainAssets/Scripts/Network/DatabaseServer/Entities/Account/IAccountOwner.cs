namespace FD.Networking.Database.Entities.Account
{
    public interface IAccountOwner
    {
        public Account AssociatedAccount { get; set; }
    }
}
