using DevourDev.MonoBase.Ai.ExExAct;
using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(fileName = "Can use Ability Enquirer", menuName = "FD/Units/Ai/Enquirers/Abilities/Can use ability (check all)")]
    public class CanUseAbilityEnquirer : UnitEnquirer
    {
        [SerializeField] private UnitAbilityObject _ability;


        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            var state = ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID];
            result = _ability.CheckCoolDown(state)
                && _ability.CheckRange(ai)
                && _ability.CheckAllStagesCosts(ai.ServerSideUnit.DynamicStatsCollection)
                && _ability.CheckTarget(ai);
        }

        public class Data : EnquirerData
        {

        }
    }
}
