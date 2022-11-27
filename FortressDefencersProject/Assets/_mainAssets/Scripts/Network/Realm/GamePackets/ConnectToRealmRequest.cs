using DevourDev.Networking;

namespace FD.Networking.Realm.GamePackets
{
    public class ConnectToRealmRequest : IPacketContent
    {
        public int UniqueID => 510;
        public byte[] Key;
        public ConnectionType ConnectionType;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Key = d.ReadBytes();
            ConnectionType = (ConnectionType)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Key);
            e.Write((int)ConnectionType);
        }
    }


}
