using DevourDev.MonoBase.Ai.ExExAct;
using FD.Global;
using FD.Units.Ai.Enquirers;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Units.Ai.Sensors
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Sensors/Units in vision range")]
    public class UnitsInVisionRangeSensor : UnitSensor
    {
        [SerializeField] private float _visionRangeMuiltiplier = 1f;
        [SerializeField] private float _visionRangeFlatBonus = 0f;
        [SerializeField, Tooltip("set UnitAllyMode.None to skip sorting")] private UnitAllyMode _allyMode;

        private Global.Rules.GameRulesObject _grm;


        private Global.Rules.GameRulesObject GRM
        {
            get
            {
                if (_grm == null)
                {
                    _grm = Global.GameManager.Instance.GameRules;
                }

                return _grm;
            }
        }


        protected override void Scan(UnitAi ai, out SensorData d)
        {
            float range = ai.ServerSideUnit.Reference.CommonStats.VisionRange * _visionRangeMuiltiplier + _visionRangeFlatBonus;

            d = ai.GetOrCreateSensorData<Data>(this);
            var sd = d as Data;
            if (_allyMode == UnitAllyMode.None)
                GRM.UnitsInRangeRule.GetUnitsInRange(ai.ServerSideUnit.transform.position, range, sd.AllUnitsInRange, ai.ServerSideUnit);
            else
                GRM.UnitsInRangeRule.GetUnitsInRange(ai.ServerSideUnit.transform.position, range, sd.AllUnitsInRange, _allyMode, ai.ServerSideUnit, ai.ServerSideUnit);
        }


        public class Data : SensorData
        {
            private List<UnitOnSceneBase> _allUnitsInRange;


            public Data()
            {
                _allUnitsInRange = new();
            }


            public List<UnitOnSceneBase> AllUnitsInRange => _allUnitsInRange;

        }

    }
}
