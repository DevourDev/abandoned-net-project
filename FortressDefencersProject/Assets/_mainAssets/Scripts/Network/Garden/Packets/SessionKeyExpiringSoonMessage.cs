namespace FD.Networking.Garden.Packets
{
    public class SessionKeyExpiringSoonMessage : IPacketContent
    {
        public int UniqueID => 100_003;

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
