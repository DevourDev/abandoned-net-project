using DevourDev.MonoBase.Ai.ExExAct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Any Unit in Vision Range")]
    public class SortedUnitsInVisionRangeEnquirer : UnitEnquirer
    {
        [SerializeField] private Sensors.UnitsInVisionRangeSensor _visionSensor;
        [SerializeField] private UnitAllyMode _allyMode;

        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            var sd = ai.GetSensorData<Sensors.UnitsInVisionRangeSensor.Data>(_visionSensor, true);
            var ted = ai.GetOrCreateEnquirerData<Data>(this);
            ed = ted;
            Enquiries.GetUnitsOfAllyMode(_allyMode, sd.AllUnitsInRange, ai.ServerSideUnit, ted.SortedUnits);
            result = ted.SortedUnits.Count > 0;
        }

        public class Data : EnquirerData
        {
            public Data()
            {
                SortedUnits = new();
            }


            public List<UnitOnSceneBase> SortedUnits { get; private set; }
        }
    }
}
