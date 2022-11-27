using DevourDev.MonoBase.Ai.ExExAct;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Units.Ai.Sensors
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Sensors/Units in Vision Range closer than Ability Target")]
    public class UnitsInAbilityRangeSensor : Sensors.UnitSensor
    {
        [SerializeField] private Abilities.UnitAbilityObject _ability;
        [SerializeField] private bool _limitToVisionRange;
        [SerializeField] private UnitsInVisionRangeSensor _visionRangeSensor;
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


        protected override void Scan(UnitAi ai, out SensorData sd)
        {
            sd = ai.GetOrCreateSensorData<Data>(this);
            if (_allyMode == UnitAllyMode.None)
                GRM.UnitsInRangeRule.GetUnitsInRange(ai.ServerSideUnit.transform.position, _ability.CastDistance, ((Data)sd).UnitsInAbilityRange, ai.ServerSideUnit);
            else
                GRM.UnitsInRangeRule.GetUnitsInRange(ai.ServerSideUnit.transform.position, _ability.CastDistance, ((Data)sd).UnitsInAbilityRange, _allyMode, ai.ServerSideUnit, ai.ServerSideUnit);
        }


        public class Data : SensorData
        {
            public List<UnitOnSceneBase> UnitsInAbilityRange { get; set; }
        }
    }
}
