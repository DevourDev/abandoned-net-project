namespace FD.Networking.Database.Entities.Account
{
    public class LogInData
    {
        /// <summary>
        /// Логин с УчЕтоМ РегиСТра
        /// </summary>
        public string SourceLogin;
        public string Login;
        /// <summary>
        /// Дважды захешенный пароль
        /// </summary>
        public string HashedPassword;
        public EmailAddress Email;


        public LogInData()
        {
            SourceLogin = "Test";
            Login = "test";
            HashedPassword = "test123";
            Email = new EmailAddress("test", "example", "com");
        }

        public LogInData(string sourceLogin, string doubleHashedPassword, EmailAddress email)
        {
            SourceLogin = sourceLogin.Trim();
            Login = SourceLogin.ToLower();
            HashedPassword = doubleHashedPassword;
            Email = email;
        }
    }
}
