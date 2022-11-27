using DevourDev.MonoBase.AbilitiesSystem;
using DevourDev.Networking;
using FD.Units;
using UnityEngine;

namespace FD.Networking.Realm.GamePackets
{
    public class NetworkUnitTarget : IPacketContent
    {
        public int UniqueID => 200_000;
        public AbilityTargetMode TargetMode;
        public int UnitTargetUniqueID;
        public Vector3 AreaTarget;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            TargetMode = (AbilityTargetMode)d.ReadByte();
            switch (TargetMode)
            {
                case AbilityTargetMode.NonTargeted:
                    break;
                case AbilityTargetMode.Area:
                    AreaTarget = d.ReadVector3();
                    break;
                case AbilityTargetMode.Agent:
                    UnitTargetUniqueID = d.ReadInt();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write((byte)TargetMode);
            switch (TargetMode)
            {
                case AbilityTargetMode.NonTargeted:
                    break;
                case AbilityTargetMode.Area:
                    e.Write(AreaTarget);
                    break;
                case AbilityTargetMode.Agent:
                    e.Write(UnitTargetUniqueID);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public Target GetTarget()
        {
            var t = new Target();
            switch (TargetMode)
            {
                case AbilityTargetMode.NonTargeted:
                    t.Set();
                    break;
                case AbilityTargetMode.Area:
                    t.Set(AreaTarget);
                    break;
                case AbilityTargetMode.Agent:
                    //Get UnitOnScene from ClientGameManager with UniqueUnitID
                    throw new System.NotImplementedException();
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return t;
        }

        public bool UpdateTarget(Target t)
        {
            switch (TargetMode)
            {
                case AbilityTargetMode.NonTargeted:
                    t.Set();
                    break;
                case AbilityTargetMode.Area:
                    t.Set(AreaTarget);
                    break;
                case AbilityTargetMode.Agent:
                    //Get UnitOnScene from ClientGameManager with UniqueUnitID
                    var nm = NetworkManager.Instance;
                    if (nm.Mode != NetworkMode.Client && nm.Mode != NetworkMode.Host)
                        throw new System.Exception("unexpected method invokation");

                    var cgm = ClientSide.Global.ClientSideGameManager.Instance;

                    if (!cgm.AllUnits.TryGetValue(UnitTargetUniqueID, out var v))
                        return false;

                    t.TrySetAgentByID(v.UniqueID);
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return true;
        }



    }

}
