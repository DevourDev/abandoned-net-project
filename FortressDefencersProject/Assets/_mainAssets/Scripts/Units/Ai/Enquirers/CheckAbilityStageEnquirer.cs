using DevourDev.MonoBase.AbilitiesSystem;
using DevourDev.MonoBase.Ai.ExExAct;
using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Ability in Stage")]
    public class CheckAbilityStageEnquirer : UnitEnquirer
    {
        [SerializeField] private UnitAbilityObject _ability;
        [SerializeField] private AbilityStage _stage;


        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            var state = ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID];
            result = state.CurrentStage == _stage;
        }


        public class Data : EnquirerData
        {

        }

    }
}
