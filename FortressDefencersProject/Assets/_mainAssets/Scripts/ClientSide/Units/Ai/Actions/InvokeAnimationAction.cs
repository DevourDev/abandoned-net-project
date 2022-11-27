using FD.ClientSide.Units.Ai;
using FD.ClientSide.Units.Ai.Actions;
using FD.Units.Ai;
using FD.Units.Ai.Actions;
using UnityEngine;

namespace FD.ClientSide.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/ClientSide/Invoke Animation")]
    public class InvokeAnimationAction : UnitAction
    {
        [SerializeField] private DevourDev.MonoBase.AnimationInvoker[] _invokers;


        public override void Act(UnitAi ai)
        {
            foreach (var invoker in _invokers)
            {
                invoker.Activate(ai.ClientSideUnit.Animator);
            }

        }
    }


}
