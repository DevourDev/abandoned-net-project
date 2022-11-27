
namespace FD.Networking.Gates.Packets
{
    public class SignUpRequest : IPacketContent
    {
        public int UniqueID => 20;
        public string Login;
        public string ClientSideHashedPassword;
        public string ClientSideHashedPasswordConfirmation;
        public string Email;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Login = d.ReadString();
            ClientSideHashedPassword = d.ReadString();
            ClientSideHashedPasswordConfirmation = d.ReadString();
            Email = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Login);
            e.Write(ClientSideHashedPassword);
            e.Write(ClientSideHashedPasswordConfirmation);
            e.Write(Email);
        }
    }

    public class SignUpResponse : IPacketContent
    {
        public int UniqueID => 21;
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
            BadLogin,
            NonUniqueLogin,
            WrongPasswords,
            BadEmail,
            Other,
        }
    }
}
