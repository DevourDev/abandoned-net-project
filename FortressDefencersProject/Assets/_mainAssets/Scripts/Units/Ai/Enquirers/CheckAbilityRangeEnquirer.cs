using DevourDev.MonoBase.Ai.ExExAct;
using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Abilities/Check range")]
    public class CheckAbilityRangeEnquirer : UnitEnquirer
    {
        [SerializeField] private UnitAbilityObject _ability;


        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            var state = ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID];
            result = _ability.CheckRange(ai);
        }

        public class Data : EnquirerData
        {

        }
    }   }
