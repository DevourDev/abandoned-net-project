using System;

namespace FD.Networking.Realm.GamePackets
{
    public class UnitStatePacket : IPacketContent
    {
        public int UniqueID => 210_004;
        public int StateID;
        /// <summary>
        /// entered State or stay in State
        /// </summary>
        public bool Entered;


        public UnitStatePacket()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            StateID = d.ReadInt();
            Entered = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(StateID);
            e.Write(Entered);
        }

        public override bool Equals(object obj)
        {
            return obj is UnitStatePacket packet &&
                   StateID == packet.StateID &&
                   Entered == packet.Entered;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UniqueID);
        }
    }

}
