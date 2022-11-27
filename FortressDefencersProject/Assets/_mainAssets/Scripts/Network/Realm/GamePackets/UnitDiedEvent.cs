namespace FD.Networking.Realm.GamePackets
{
    public class UnitDiedEvent : IPacketContent
    {
        public int UniqueID => 300_101;
        public int UnitUniqueID;
        public int KillerUnitUniqueID;


        public UnitDiedEvent()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            UnitUniqueID = d.ReadInt();
            KillerUnitUniqueID = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(UnitUniqueID);
            e.Write(KillerUnitUniqueID);
        }
    }

}
