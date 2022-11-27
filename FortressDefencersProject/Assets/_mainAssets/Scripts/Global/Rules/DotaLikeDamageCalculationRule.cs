using System;
using UnityEngine;
using FD.Units;
using FD.Units.Stats;

namespace FD.Global.Rules
{
    [CreateAssetMenu(menuName = "FD/Game Rules/Damage Calculation/Dota-like")]
    public class DotaLikeDamageCalculationRule : DamageCalculationRuleBase
    {
        [SerializeField] private float _armorFormulaBase = 1.0f;
        [SerializeField] private float _armorFormulaFactor = 0.06f;

        public float ArmorFormulaBase => _armorFormulaBase;
        public float ArmorFormulaFactor => _armorFormulaFactor;


        public override float CalculateFinalDamage(DamageTypeObject dmgType, float dmgValue,
            ArmorTypeObject armorType, float armorValue)
        {

            float armorTypedamageMultiplier;
            foreach (var item in armorType.Overrides)
            {
                if (item.DamageType.UniqueID == dmgType.UniqueID)
                {
                    armorTypedamageMultiplier = item.Multiplier;
                    goto DamageTypeMultiplierOverrided;
                }
            }
            armorTypedamageMultiplier = armorType.DefaultDamageMultiplier;

        DamageTypeMultiplierOverrided:

            var armorValueDmgMultiplier = 1 - (ArmorFormulaFactor * armorValue) / (ArmorFormulaBase + ArmorFormulaFactor * Math.Abs(armorValue));

            return dmgValue * armorTypedamageMultiplier * armorValueDmgMultiplier;
        }
    }
}
