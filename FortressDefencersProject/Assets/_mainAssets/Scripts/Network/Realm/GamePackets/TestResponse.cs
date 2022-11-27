namespace FD.Networking.Realm.GamePackets
{
    public class TestResponse : IPacketContent
    {
        public int UniqueID => 991;
        public bool Result;
        public string EchoString;
        public int VectorsAmount;
        public UnityEngine.Vector3[] Vector3s;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            EchoString = d.ReadString();
            VectorsAmount = d.ReadInt();
            Vector3s = d.ReadVector3s();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            e.Write(EchoString);
            e.Write(VectorsAmount);
            e.Write(Vector3s);
        }
    }


}
