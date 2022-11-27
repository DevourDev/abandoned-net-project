using DevourDev.MonoBase;
using DevourDev.MonoBase.Multilanguage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Units.Stats
{
    [CreateAssetMenu(menuName = "FD/Units/Armor/New Armor Type Object")]
    public class ArmorTypeObject : GameDatabaseElement
    {
        [SerializeField] private InternationalString _armorTypeName;
        [SerializeField] private float _defaultDamageMultiplier = 1;
        [SerializeField] private List<DamageMultiplierOverride> _overrides;



        public InternationalString ArmorStatName => _armorTypeName;
        public float DefaultDamageMultiplier => _defaultDamageMultiplier;
        public List<DamageMultiplierOverride> Overrides => _overrides;



        [Serializable]
        public class DamageMultiplierOverride
        {
            [SerializeField] private DamageTypeObject _damageType;
            [SerializeField] private float _multiplier = 1;


            public DamageTypeObject DamageType => _damageType;
            public float Multiplier => _multiplier;

            public override bool Equals(object obj)
            {
                return obj is DamageMultiplierOverride @override &&
                       EqualityComparer<DamageTypeObject>.Default.Equals(_damageType, @override._damageType);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_damageType);
            }
        }
    }
}
