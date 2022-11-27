namespace FD.Networking.Gates.Packets
{
    public class LogInRequest : IPacketContent
    {
        public int UniqueID => 10;
        public string Login;
        public string ClientSideHashedPassword;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Login = d.ReadString();
            ClientSideHashedPassword = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Login);
            e.Write(ClientSideHashedPassword);
        }
    }

    public class LogInResponse : IPacketContent
    {
        public int UniqueID => 11;
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
            WrongLogin,
            WrongPassword,
            Other,
        }
    }
}
