using DevourDev.MonoBase.AbilitiesSystem;
using DevourDev.Networking;
using FD.Networking;
using FD.Networking.Realm.GamePackets;
using UnityEngine;

namespace FD.Units
{
    public class Target : TargetBase<UnitOnSceneBase>, System.IEquatable<Target>, System.IEquatable<NetworkUnitTarget>
    {
        private ClientSide.Units.UnitOnSceneClientSide _clientSideAgent;

        private bool _hasClientSideAgent;
        private bool _hasServerSideAgent;

        public bool HasClientSideAgent => _hasClientSideAgent;
        public bool HasServerSideAgent => _hasServerSideAgent;

        public ClientSide.Units.UnitOnSceneClientSide ClientSideAgent => _clientSideAgent;
        public UnitOnSceneBase ServerSideAgent => Agent;


        public bool AnyAgentExists
        {
            get
            {
                if (_hasClientSideAgent && ClientSideAgent != null)
                    return true;

                if (_hasServerSideAgent && ServerSideAgent != null)
                    return true;

                return false;
            }
        }

        protected int GetAnyAgentID()
        {
            if (_hasServerSideAgent)
                return Agent.UniqueID;

            if (_hasClientSideAgent)
                return ClientSideAgent.UniqueID;

            return -1;
        }

        public bool TryGetAnyAgentID(out int id)
        {
            id = GetAnyAgentID();
            return id != -1;
        }

        public bool TrySetAgentByID(int id)
        {
            var nm = NetworkManager.Instance;
            switch (nm.Mode)
            {
                case NetworkMode.Server:
                    var gm = Global.GameManager.Instance;

                    if (gm.TryGetUnitByID(id, out var a))
                    {
                        Set(a);
                        _hasServerSideAgent = true;
                        Mode = AbilityTargetMode.Agent;
                        return true;
                    }

                    return false;

                case NetworkMode.Client:
                    var cgm = ClientSide.Global.ClientSideGameManager.Instance;
                    if (cgm.AllUnits.TryGetValue(id, out var csA))
                    {
                        Set(csA);
                        _hasClientSideAgent = true;
                        Mode = AbilityTargetMode.Agent;
                        return true;
                    }

                    return false;

                case NetworkMode.Host:
                    gm = Global.GameManager.Instance;

                    if (gm.TryGetUnitByID(id, out a))
                    {
                        Set(a);
                        _hasClientSideAgent = true;
                        Mode = AbilityTargetMode.Agent;
                    }
                    else
                    {
                        return false;
                    }
                    cgm = ClientSide.Global.ClientSideGameManager.Instance;
                    if (cgm.AllUnits.TryGetValue(id, out csA))
                    {
                        Set(csA);
                        _hasClientSideAgent = true;
                        Mode = AbilityTargetMode.Agent;
                        return true;
                    }

                    return false;
                default:
                    throw new System.Exception($"Unexpected enum value: {nameof(NetworkMode)}.{nm.Mode}");
            }
        }


        public override void Set(UnitOnSceneBase a)
        {
            base.Set(a);
            _hasServerSideAgent = true;
        }
        public void Set(ClientSide.Units.UnitOnSceneClientSide csA)
        {
            _clientSideAgent = csA;
            _hasClientSideAgent = true;
            Mode = AbilityTargetMode.Agent;
        }

        public void Set(Target t)
        {
            if (t.Mode == AbilityTargetMode.Agent)
            {
                if (t.HasClientSideAgent)
                {
                    _clientSideAgent = t._clientSideAgent;
                    _hasClientSideAgent = true;
                }

                if (t.HasServerSideAgent)
                {
                    Agent = t.Agent;
                    _hasServerSideAgent = true;
                }
            }
            else
            {
                base.Set(t);
            }
        }


        public override bool TryGetPoint(Vector3 ownerPos, out Vector3 point)
        {
            switch (Mode)
            {
                case AbilityTargetMode.Area:
                    point = Area;
                    break;
                case AbilityTargetMode.Agent:

                    if (_hasClientSideAgent)
                        point = Agent.transform.position;
                    else if (_hasClientSideAgent)
                        point = ClientSideAgent.transform.position;
                    else
                        goto default;

                    break;
                case AbilityTargetMode.NonTargeted:
                    point = ownerPos;
                    break;
                default:
                    point = default;
                    return false;
            }

            return true;
        }


        public NetworkUnitTarget ToPacketContent()
        {
            NetworkUnitTarget nt = new();
            nt.TargetMode = Mode;
            switch (Mode)
            {
                case AbilityTargetMode.NonTargeted:
                    break;
                case AbilityTargetMode.Area:
                    nt.AreaTarget = Area;
                    break;
                case AbilityTargetMode.Agent:
                    if (_hasServerSideAgent)
                    {
                        nt.UnitTargetUniqueID = Agent.UniqueID;
                    }
                    else if (_hasClientSideAgent)
                    {
                        nt.UnitTargetUniqueID = ServerSideAgent.UniqueID;
                    }
                    else
                    {
                        throw new System.Exception("Target mode is Agent, but neither ServerSide nor ClientSide Agents are setted.");
                    }
                    break;
                default:
                    throw new System.NotImplementedException();

            }

            return nt;
        }

        public bool Equals(NetworkUnitTarget other)
        {
            if (Mode != other.TargetMode)
                return false;

            return Mode switch
            {
                AbilityTargetMode.NonTargeted => true,
                AbilityTargetMode.Area => Area == other.AreaTarget,
                AbilityTargetMode.Agent => TryGetAnyAgentID(out var id) && id == other.UniqueID,
                _ => false,
            };
        }

        public bool Equals(Target other)
        {
            if (Mode != other.Mode)
                return false;

            return Mode switch
            {
                AbilityTargetMode.NonTargeted => true,
                AbilityTargetMode.Area => Area == other.Area,
                AbilityTargetMode.Agent => CheckAgentTargetsEquality(other),
                _ => false,
            };
        }

        private bool CheckAgentTargetsEquality(Target other)
        {
            if (HasClientSideAgent != other.HasClientSideAgent
                || HasServerSideAgent == other.HasServerSideAgent)
                return false;

            if (HasClientSideAgent)
            {
                if (ClientSideAgent.UniqueID != other.ClientSideAgent.UniqueID)
                    return false;
            }

            if (HasServerSideAgent)
            {
                if (Agent.UniqueID != other.Agent.UniqueID)
                    return false;
            }

            return true;
        }
    }
}

