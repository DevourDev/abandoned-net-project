#define USE_INLINING
//#undef USE_INLINING

using System.Runtime.CompilerServices;
using UnityEngine;

namespace DevourDev.MonoBase.AbilitiesSystem
{
    public abstract class TargetBase<AgentTarget> where AgentTarget : MonoBehaviour
    {
        private AbilityTargetMode _mode;
        private Vector3 _area;
        private AgentTarget _agent;


        public AbilityTargetMode Mode { get => _mode; protected set => _mode = value; }
        protected Vector3 Area { get => _area; set => _area = value; }
        protected AgentTarget Agent { get => _agent; set => _agent = value; }


#if USE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public virtual void Set(AgentTarget a)
        {
            Agent = a;
            Mode = AbilityTargetMode.Agent;
        }
#if USE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Set(Vector3 a)
        {
            Area = a;
            Mode = AbilityTargetMode.Area;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Set(TargetBase<AgentTarget> t)
        {
            Mode = t.Mode;

            switch (t.Mode)
            {
                case AbilityTargetMode.Area:
                    Area = t.Area;
                    break;
                case AbilityTargetMode.Agent:
                    Agent = t.Agent;
                    break;
                default:
                    break;
            }
        }

#if USE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Set()
        {
            Mode = AbilityTargetMode.NonTargeted;
        }

#if USE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool TryGetPoint(AgentTarget owner, out Vector3 point)
        {
            switch (Mode)
            {
                case AbilityTargetMode.Area:
                    point = Area;
                    break;
                case AbilityTargetMode.Agent:
                    if (Agent == null)
                        goto default;

                    point = Agent.transform.position;
                    break;
                case AbilityTargetMode.NonTargeted:
                    point = owner.transform.position;
                    break;
                default:
                    point = default;
                    return false;
            }

            return true;
        }

#if USE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public virtual bool TryGetPoint(Vector3 ownerPos, out Vector3 point)
        {
            switch (Mode)
            {
                case AbilityTargetMode.Area:
                    point = Area;
                    break;
                case AbilityTargetMode.Agent:
                    if (Agent == null)
                        goto default;

                    point = Agent.transform.position;
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
    }
}