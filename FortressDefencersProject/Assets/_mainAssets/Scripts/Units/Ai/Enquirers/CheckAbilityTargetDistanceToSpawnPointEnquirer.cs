using DevourDev.MonoBase.Ai.ExExAct;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Ability Target Distance to Spawn Point Comparer Enquirer")]
    public class CheckAbilityTargetDistanceToSpawnPointEnquirer : UnitEnquirer
    {
        [SerializeField] private Abilities.UnitAbilityObject _abilityObject;
        [SerializeField] private Sensors.SpawnedPositionSensor _spawnedPositionSensor;

        [SerializeField] private FloatEnquiryConditionEnum _condition;
        [SerializeField] private float _comparer;

        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            var d = ed as Data;
            var spawnPos = ai.GetSensorData<Sensors.SpawnedPositionSensor.Data>(_spawnedPositionSensor, true).SpawnPos;
            var aState = ai.ServerSideUnit.AbilitiesCollection.Collection[_abilityObject.UniqueID];
            if(aState.Target.TryGetPoint(ai.ServerSideUnit, out var p))
            {
                d.Distance = Vector3.Distance(ai.ServerSideUnit.transform.position, p);
            }
            else
            {
                result = false;
                return;
            }

            result = _condition switch
            {
                FloatEnquiryConditionEnum.LessThan => d.Distance < _comparer,
                FloatEnquiryConditionEnum.LessOrEqualTo => d.Distance <= _comparer,
                FloatEnquiryConditionEnum.EqualTo => d.Distance == _comparer,
                FloatEnquiryConditionEnum.MoreOrEqualTo => d.Distance >= _comparer,
                FloatEnquiryConditionEnum.MoreThan => d.Distance > _comparer,
                _ => false,
            };
        }


        public class Data : EnquirerData
        {
            public float Distance { get; set; }
        }

      
    }
}
