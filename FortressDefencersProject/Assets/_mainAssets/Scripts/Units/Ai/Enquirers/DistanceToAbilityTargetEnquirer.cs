using DevourDev.MonoBase.Ai.ExExAct;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Distance to Ability Target")]
    public class DistanceToAbilityTargetEnquirer : UnitEnquirer
    {
        [SerializeField] private Abilities.UnitAbilityObject _abilityObject;
        [SerializeField] private FloatEnquiryConditionEnum _condition;
        [SerializeField] private float _comparer;

        private float? _sqrComparer;

        private float SqrComparer
        {
            get
            {
                if (!_sqrComparer.HasValue)
                {
                    _sqrComparer = _comparer * _comparer;
                }

                return _sqrComparer.Value;
            }
        }

        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            var d = ed as Data;
            var aState = GetAbilityState(ai, _abilityObject);
            result = false;
            if (aState.Target.TryGetPoint(ai.ServerSideUnit, out var p))
            {
                d.SqrDistance = (ai.ServerSideUnit.transform.position - p).sqrMagnitude;
                result = _condition switch
                {
                    FloatEnquiryConditionEnum.LessThan => d.SqrDistance < SqrComparer,
                    FloatEnquiryConditionEnum.LessOrEqualTo => d.SqrDistance <= SqrComparer,
                    FloatEnquiryConditionEnum.EqualTo => d.SqrDistance == SqrComparer,
                    FloatEnquiryConditionEnum.MoreOrEqualTo => d.SqrDistance >= SqrComparer,
                    FloatEnquiryConditionEnum.MoreThan => d.SqrDistance > SqrComparer,
                    _ => false,
                };
            }
        }


        public class Data : EnquirerData
        {
            public float SqrDistance { get; set; }
        }
    }
}
