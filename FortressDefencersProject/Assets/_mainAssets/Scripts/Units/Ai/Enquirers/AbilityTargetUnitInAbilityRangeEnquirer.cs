using DevourDev.MonoBase.Ai.ExExAct;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Unit is in Ability Range")]
    public class AbilityTargetUnitInAbilityRangeEnquirer : Enquirers.UnitEnquirer
    {
        [SerializeField] private Abilities.UnitAbilityObject _ability;


        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            var aState = ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID];
            result = false;
            if (aState.Target.TryGetPoint(ai.ServerSideUnit, out var p))
            {
                result = aState.Reference.CheckRange(ai);
                //result = Vector3.Distance(ai.ServerSideUnit.transform.position, p) <= _ability.CastDistance;
            }
        }


        public class Data : EnquirerData
        {
        }
    }
}
