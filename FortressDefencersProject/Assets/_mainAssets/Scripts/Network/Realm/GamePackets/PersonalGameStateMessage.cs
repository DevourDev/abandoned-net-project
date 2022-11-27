namespace FD.Networking.Realm.GamePackets
{
    public class PersonalGameStateMessage : IPacketContent
    {
        public int UniqueID => 500_004;

        public int Coins;
        public int[] ShowCase;


        public PersonalGameStateMessage()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Coins = d.ReadInt();
            ShowCase = d.ReadInts();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Coins);
            e.Write(ShowCase);
        }
    }


}
