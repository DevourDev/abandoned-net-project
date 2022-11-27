namespace FD.Networking.Packets
{
    public class HeartBeatResponse : IPacketContent
    {
        public int UniqueID => 1_000_011;

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
