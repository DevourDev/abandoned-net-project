using FD.Global;

namespace FD.Networking.Garden.Packets
{
    public class EnterFindGameQueueRequest : IPacketContent
    {
        public int UniqueID => 20;
        public byte[] SessionKey;
        public GameMode Mode;


        public EnterFindGameQueueRequest()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SessionKey = d.ReadBytes();
            Mode = (GameMode)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SessionKey);
            e.Write((int)Mode);
        }
    }

    public class EnterFindGameQueueResponse : IPacketContent
    {
        public int UniqueID => 21;
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
            WrongSessionKey,
            AccountPenalty,
            GameModeUnavailableServerSide,
            Other,
        }
    }
}
