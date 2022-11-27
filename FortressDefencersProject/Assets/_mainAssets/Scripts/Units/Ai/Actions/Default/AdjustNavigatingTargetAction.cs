using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Navigation/Adjust Target's Position")]
    public class AdjustNavigatingTargetAction : UnitAction
    {
        [SerializeField] private Abilities.UnitAbilityObject _abilityWithNavigatingTarget;
        [SerializeField] private Actions.UnitAction _noTargetAction;
        //[SerializeField] private bool _compareSqrDistances = true;
        [SerializeField] private float _sqrErrorValue = 0.3f * 0.3f;


        public override void Act(UnitAi ai)
        {
            var target = ai.ServerSideUnit.AbilitiesCollection.Collection[_abilityWithNavigatingTarget.UniqueID].Target;

            if (target.Mode != DevourDev.MonoBase.AbilitiesSystem.AbilityTargetMode.Agent)
                return;

            if (!target.TryGetPoint(ai.ServerSideUnit.transform.position, out var curPos))
            {
                if (_noTargetAction == null)
                {
                    ai.ServerSideUnit.NavMeshAgent.isStopped = true;
                    return;
                }
                else
                {
                    _noTargetAction.Act(ai);
                    return;
                }
            }

            var nma = ai.ServerSideUnit.NavMeshAgent;
            var prevPos = nma.destination;
            var sqrD = (prevPos - curPos).sqrMagnitude;

            if (_sqrErrorValue < sqrD)
                nma.SetDestination(curPos);
        }
    }
}
//    bool needToRepath;
//    if (_compareSqrDistances)
//        needToRepath = ByComparingSqrDistances(prevPos, curPos);
//    else
//        needToRepath = ByVectorsSubtrackting(prevPos, curPos);

//    if (needToRepath)
//    {
//        nma.SetDestination(curPos);
//    }
//}


//private bool ByComparingSqrDistances(Vector3 prevPos, Vector3 curPos)
//{
//    var sqrD = (prevPos - curPos).sqrMagnitude;
//    return _sqrErrorValue < sqrD;
//}
