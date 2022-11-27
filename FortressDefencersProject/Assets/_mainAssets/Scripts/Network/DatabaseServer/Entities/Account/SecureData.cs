namespace FD.Networking.Database.Entities.Account
{
    public class SecureData
    {
        public LogInData LogInData;
        public SecureHistory History;


        public SecureData()
        {
            LogInData = new();
            History = new();
        }

        public SecureData(LogInData logInData, SecureHistory history)
        {
            LogInData = logInData;
            History = history;
        }
    }
}
