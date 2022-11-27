using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Navigation/Start Navigating")]
    public class StartNavigatingAction : UnitAction
    {
        [SerializeField, Tooltip("to take target from")] private UnitAbilityObject _movingAbility;
        public override void Act(UnitAi ai)
        {
            if (_movingAbility != null)
            {
                var aState = ai.ServerSideUnit.AbilitiesCollection.Collection[_movingAbility.UniqueID];
                if (aState.Target.TryGetPoint(ai.ServerSideUnit, out var p))
                {
                    ai.ServerSideUnit.NavMeshAgent.destination = p;
                }
            }

            ai.ServerSideUnit.NavMeshAgent.isStopped = false;
        }
    }


}
