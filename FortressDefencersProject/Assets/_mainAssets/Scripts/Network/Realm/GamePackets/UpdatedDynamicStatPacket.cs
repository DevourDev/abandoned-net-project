namespace FD.Networking.Realm.GamePackets
{
    public class UpdatedDynamicStatPacket : IPacketContent
    {
        public int UniqueID => 300_001;
        public int StatID;
        public float Current;
        public bool BoundsChanged; //rename
        public float MaxValue;
        public float MinValue;
        public float RegenValue;


        public UpdatedDynamicStatPacket()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            StatID = d.ReadInt();
            Current = d.ReadSingle();
            BoundsChanged = d.ReadBool();

            if (BoundsChanged)
            {
                MaxValue = d.ReadSingle();
                MinValue = d.ReadSingle();
                RegenValue = d.ReadSingle();
            }
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(StatID);
            e.Write(Current);
            e.Write(BoundsChanged);

            if (BoundsChanged)
            {
                e.Write(MaxValue);
                e.Write(MinValue);
                e.Write(RegenValue);
            }
        }
    }

}
