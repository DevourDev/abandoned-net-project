using FD.Networking.Database.Entities.Account;

namespace FD.Networking.Database.Packets
{
    public class HandleSignUpRequest : IPacketContent
    {
        public int UniqueID => 10_020;
        public string Login;
        public string ReHashedPassword;
        public EmailAddress Email;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Login = d.ReadString();
            ReHashedPassword = d.ReadString();
            Email = d.ReadXml<EmailAddress>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Login);
            e.Write(ReHashedPassword);
            e.WriteXml(Email);
        }
    }

    public class HandleSignUpResponse : IPacketContent
    {
        public int UniqueID => 10_021;
        public bool Result;
        public byte[] SessionKey;
        public Error FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (Result)
                SessionKey = d.ReadBytes();
            else
                FailReason = (Error)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (Result)
                e.Write(SessionKey);
            else
                e.Write((int)FailReason);
        }

        public enum Error
        {
            UnavailableLogin,
            Other,
        }
    }
}