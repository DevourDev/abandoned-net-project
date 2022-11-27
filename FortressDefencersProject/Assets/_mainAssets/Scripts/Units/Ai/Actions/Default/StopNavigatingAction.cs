using System;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Navigation/Stop Navigating")]
    public class StopNavigatingAction : UnitAction
    {
        public override void Act(UnitAi ai)
        {
            ai.ServerSideUnit.NavMeshAgent.isStopped = true;
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
