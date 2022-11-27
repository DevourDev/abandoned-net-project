using DevourDev.Networking;

namespace FD.Networking.Garden.Packets
{
    public class ConnectToGardenRequest : IPacketContent
    {
        public int UniqueID => 10;
        public byte[] SessionKey;
        /// <summary>
        /// Пользователь хочет получить данный тип подключения.
        /// (Requester of MessageListener)
        /// </summary>
        public ConnectionType ConnectionType;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SessionKey = d.ReadBytes();
            ConnectionType = (ConnectionType)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SessionKey);
            e.Write((int)ConnectionType);
        }
    }

    public class ConnectToGardenResponse : IPacketContent
    {
        public int UniqueID => 11;
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
            Other
        }
    }
}
