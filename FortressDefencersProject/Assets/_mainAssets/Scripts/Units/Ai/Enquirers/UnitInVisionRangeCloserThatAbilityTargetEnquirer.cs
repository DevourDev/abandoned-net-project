using DevourDev.MonoBase.Ai.ExExAct;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{


    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Unit in Vision Range closer than Ability Target")]
    public class UnitInVisionRangeCloserThatAbilityTargetEnquirer : UnitEnquirer
    {
        [SerializeField] private Sensors.UnitsInVisionRangeSensor _visionSensor;
        [SerializeField] private bool _useEnquirer;
        [SerializeField] private Enquirers.SortedUnitsInVisionRangeEnquirer _sortedUnitsEnquirer;
        [SerializeField] private Abilities.UnitAbilityObject _ability;


        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);

            List<UnitOnSceneBase> units;
            if (_useEnquirer)
            {
                var sortedUnitsED = ai.GetEnquirerData<Enquirers.SortedUnitsInVisionRangeEnquirer.Data>(_sortedUnitsEnquirer, true);
                if (!sortedUnitsED.EnquiryResult)
                {
                    result = false;
                    return;
                }

                units = sortedUnitsED.SortedUnits;

            }
            else
            {
                var sd = ai.GetSensorData<Sensors.UnitsInVisionRangeSensor.Data>(_visionSensor, true);
                if (sd.AllUnitsInRange.Count == 0)
                {
                    result = false;
                    return;
                }

                units = sd.AllUnitsInRange;
            }

            var abilityState = ai.ServerSideUnit.AbilitiesCollection.Collection[_ability.UniqueID];
            var (closestUnit, sqrDist) = Enquiries.GetClosestUnitTo(ai.ServerSideUnit.transform.position, units);
            result = closestUnit != abilityState.Target.ServerSideAgent;

            if (result)
            {
                ((Data)ed).ClosestUnit = closestUnit;
                ((Data)ed).SqrDistance = sqrDist;
            }
        }


        public class Data : EnquirerData
        {
            public UnitOnSceneBase ClosestUnit { get; set; }
            public float SqrDistance { get; set; }
        }
    }
}
