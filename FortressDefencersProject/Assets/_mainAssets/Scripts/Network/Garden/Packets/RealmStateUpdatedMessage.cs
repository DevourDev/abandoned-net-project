namespace FD.Networking.Garden.Packets
{
    public class RealmStateUpdatedMessage : IPacketContent
    {
        public int UniqueID => 33;
        public RealmState RealmState;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            RealmState = (RealmState)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write((int)RealmState);
        }
    }
}
