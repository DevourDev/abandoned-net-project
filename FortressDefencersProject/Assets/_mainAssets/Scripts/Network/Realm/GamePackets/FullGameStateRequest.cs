namespace FD.Networking.Realm.GamePackets
{
    public class FullGameStateRequest : IPacketContent
    {
        public int UniqueID => 520;


        public FullGameStateRequest()
        {

        }


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
