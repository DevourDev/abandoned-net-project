using UnityEngine;

namespace FD.Networking.Realm.GamePackets
{

    public class TransformPacket : IPacketContent
    {
        public int UniqueID => 300_000;
        public Vector3 Position;
        public float YRotation;


        public TransformPacket()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Position = d.ReadVector3();
            YRotation = d.ReadSingle();

        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Position);
            e.Write(YRotation);
        }
    }

}
