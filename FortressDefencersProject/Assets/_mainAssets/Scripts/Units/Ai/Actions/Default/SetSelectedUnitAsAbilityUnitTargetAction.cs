using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Selected Unit as Ability Unit Target")]
    public class SetSelectedUnitAsAbilityUnitTargetAction : SetAbilityTargetActionBase
    {
        [SerializeField] private UnitOnSceneBase _target;


        public override void Act(UnitAi ai)
        {
            SetTarget(ai, _target);
        }
    }

}
