using DevourDev.MonoBase.AbilitiesSystem;
using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Abilities/Set Ability Stage")]
    public class SetAbilityStageAction : UnitAction
    {
        [SerializeField] private UnitAbilityObject _ability;
        [SerializeField] private AbilityStage _stage;


        public override void Act(UnitAi ai)
        {
            //mb add ClientSide support?
            ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID].SetStage(_stage);
        }
    }
}
