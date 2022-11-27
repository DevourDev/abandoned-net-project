namespace FD.Networking.Database.Packets
{
    public class RegistrateGameOverRequest : IPacketContent
    {
        public int UniqueID => 10_100;
        public long AccID;
        public long MatchID;
        public MatchResult MatchResult; //todo: get all match data from match. Access to match from MatchID. Match-entity should be containing in MatchesDatabase
        public int MmrChange;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            AccID = d.ReadInt64();
            MatchID = d.ReadInt64();
            MatchResult = (MatchResult)d.ReadByte();
            MmrChange = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(AccID);
            e.Write(MatchID);
            e.Write((byte)MatchResult);
            e.Write(MmrChange);

        }
    }

    public class RegistrateGameOverResponse : IPacketContent
    {
        public int UniqueID => 10_101;
        public bool Result;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }

    public class HandleLogInRequest : IPacketContent
    {
        public int UniqueID => 10_010;
        public string Login;
        public string ReHashedPassword;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Login = d.ReadString();
            ReHashedPassword = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Login);
            e.Write(ReHashedPassword);
        }
    }

    public class HandleLogInResponse : IPacketContent
    {
        public int UniqueID => 10_011;
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
            AccountNotFound,
            WrongPassword,
            Other,
        }
    }
}
