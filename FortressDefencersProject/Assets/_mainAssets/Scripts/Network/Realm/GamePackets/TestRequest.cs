namespace FD.Networking.Realm.GamePackets
{
    public class TestRequest : IPacketContent
    {
        public int UniqueID => 990;
        public string EchoString;
        public UnityEngine.Vector3[] Vector3s;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            EchoString = d.ReadString();
            Vector3s = d.ReadVector3s();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(EchoString);
            e.Write(Vector3s);
        }
    }


}
