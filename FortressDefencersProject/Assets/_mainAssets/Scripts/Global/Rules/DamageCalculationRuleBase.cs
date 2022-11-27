using FD.Units.Stats;
using UnityEngine;

namespace FD.Global.Rules
{
    public abstract class DamageCalculationRuleBase : ScriptableObject
    {
        public abstract float CalculateFinalDamage(DamageTypeObject dmgType, float dmgValue,
            ArmorTypeObject armorType, float armorValue);
    }
}
