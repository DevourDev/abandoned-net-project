using DevourDev.MonoBase.AbilitiesSystem;

namespace FD.Networking.Realm.GamePackets
{
    public class AbilityStagePacket : IPacketContent
    {
        public int UniqueID => 210_001;
        public int AbilityID;
        public AbilityStage Stage;
        public bool TargetChanged;
        public NetworkUnitTarget Target;


        public AbilityStagePacket()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            AbilityID = d.ReadInt();
            Stage = (AbilityStage)d.ReadByte();
            TargetChanged = d.ReadBool();

            if (TargetChanged)
                Target = d.ReadDecodable<NetworkUnitTarget>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(AbilityID);
            e.Write((byte)Stage);
            e.Write(TargetChanged);

            if (TargetChanged)
                e.Write(Target);
        }
    }

}
