using FD.Units.Stats;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Aggro/Deal Damage")]
    public class DealDamageAction : UnitAction
    {
        [SerializeField] private Abilities.UnitAbilityObject _attackAbility;

        [SerializeField, InspectorName("Damage Amount")] private float _dmg;
        [SerializeField, InspectorName("Damage Type")] private DamageTypeObject _dmgType;


        public override void Act(UnitAi ai)
        {
            if (!_attackAbility.CheckTarget(ai))
                return;

            var victim = ai.ServerSideUnit.AbilitiesCollection.Collection[_attackAbility.UniqueID].Target.ServerSideAgent;
            victim.TakeDamage(_dmgType, _dmg, ai.ServerSideUnit);
        }
    }
}
