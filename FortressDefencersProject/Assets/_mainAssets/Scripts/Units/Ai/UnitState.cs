using DevourDev.MonoBase.Ai.ExExAct;
using DevourDev.Networking;
using FD.Units.Ai.Actions;
using UnityEngine;

namespace FD.Units.Ai
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/New State")]
    public class UnitState : AiStateBase<UnitAi, ConditionalUnitActions>
    {
        [SerializeField] private ConditionalUnitActions[] _enteringClientSideActions;
        [SerializeField] private ConditionalUnitActions[] _stayingClientSideActions;


        protected override void HandleEntering(UnitAi ai)
        {
            var nm = Networking.NetworkManager.Instance;

            if (nm.Mode == NetworkMode.Server
                || nm.Mode == NetworkMode.Host)
            {
                base.HandleEntering(ai);
            }

            if (nm.Mode == NetworkMode.Client
                || nm.Mode == NetworkMode.Host)
            {
                foreach (var a in _enteringClientSideActions)
                {
                    a.Evaluate(ai);
                }
            }
        }

        protected override void HandleStaying(UnitAi ai)
        {
            var nm = Networking.NetworkManager.Instance;

            if (nm.Mode == NetworkMode.Server
                || nm.Mode == NetworkMode.Host)
            {
                base.HandleStaying(ai);
            }

            if (nm.Mode == NetworkMode.Client
                || nm.Mode == NetworkMode.Host)
            {
                foreach (var a in _stayingClientSideActions)
                {
                    a.Evaluate(ai);
                }
            }
        }
    }

}
