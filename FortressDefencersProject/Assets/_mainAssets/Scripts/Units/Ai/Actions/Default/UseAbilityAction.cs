using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Use Ability")]
    public class UseAbilityAction : UnitAction
    {
        [SerializeField, Tooltip("can cause KeyNotFoundException")] private UnitAbilityObject _ability;


        public override void Act(UnitAi ai)
        {
            ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID].SetStage(_ability.StagesSettingsArr[0].Stage);
        }
    }
}
