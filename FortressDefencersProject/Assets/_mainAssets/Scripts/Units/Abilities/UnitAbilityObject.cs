using DevourDev.MonoBase.AbilitiesSystem;
using FD.Units.Ai;
using FD.Units.Stats;
using UnityEngine;

namespace FD.Units.Abilities
{
    public abstract class UnitAbilityObject : AbilityObjectBase<DynamicStatObject, UnitAi, UnitAbilityState, DynamicStatsCollection, ConditionalUnitActions, UnitAbilityObject, UnitAbilityStageSettings>
    {
        public virtual bool CheckRange(UnitAi ai)
        {
            var abState = ai.ServerSideUnit.AbilitiesCollection.Collection[UniqueID];
            if (abState.Target.TryGetPoint(ai.ServerSideUnit, out var targetPoint))
            {
                return (ai.ServerSideUnit.transform.position - targetPoint).sqrMagnitude <= SqrCastDistance;
            }

            return false;
        }

        public virtual bool CheckTarget(UnitAi ai)
        {
            var aState = ai.ServerSideUnit.AbilitiesCollection.Collection[UniqueID];
            var target = aState.Target;

            if (target.Mode != TargetMode)
                return false;

            if (target.Mode == AbilityTargetMode.Agent)
                return target.AnyAgentExists;

            return true;
        }
    }
}
