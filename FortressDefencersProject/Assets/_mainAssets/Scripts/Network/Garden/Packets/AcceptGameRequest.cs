namespace FD.Networking.Garden.Packets
{

    public class DisconnectRequest : IPacketContent
    {
        public int UniqueID => 10_000;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }
    public class DisconnectResponse : IPacketContent
    {
        public int UniqueID => 10_001;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }

    public class TestGardenPacketMessage : IPacketContent
    {
        public int UniqueID => 228;
        public string Message;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Message = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Message);
        }
    }

    public class TestGardenEchoRequest : IPacketContent
    {
        public int UniqueID => 320;
        public string Message;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Message = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Message);
        }
    }
    public class TestGardenEchoResponse : IPacketContent
    {
        public int UniqueID => 321;
        public string Message;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Message = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Message);
        }
    }

    public class AcceptGameRequest : IPacketContent
    {
        public int UniqueID => 30;


        public AcceptGameRequest()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }
    public class AcceptGameResponse : IPacketContent
    {
        public int UniqueID => 31;
        public bool Result;
        public Error FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (!Result)
                FailReason = (Error)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (!Result)
                e.Write((int)FailReason);
        }

        public enum Error
        {
            Timeout,
            OtherPlayerDeclined,
            Other
        }
    }
}
