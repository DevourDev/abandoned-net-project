namespace FD.Networking.Garden.Packets
{
    public class LobbyDestroyedMessage : IPacketContent
    {
        public int UniqueID => 34;

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
}
