namespace FD.Networking.Realm.GamePackets
{
    public class DynamicStatAllValuesPacket : IPacketContent
    {
        public int UniqueID => 210_000;
        public int StatID;
        public float Max;
        public float Min;
        public float Current;
        public float Regen;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            StatID = d.ReadInt();
            Max = d.ReadSingle();
            Min = d.ReadSingle();
            Current = d.ReadSingle();
            Regen = d.ReadSingle();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(StatID);
            e.Write(Max);
            e.Write(Min);
            e.Write(Current);
            e.Write(Regen);
        }
    }
}
