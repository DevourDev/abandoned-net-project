using DevourDev.Base;
using DevourDev.MonoBase;
using DevourDev.MonoBase.Multilanguage;
using System;
using UnityEngine;

namespace FD.Units.Stats
{
    [CreateAssetMenu(menuName = "FD/Units/Damage/New Damage Type Object")]
    public class DamageTypeObject : GameDatabaseElement
    {
        [SerializeField] private InternationalString _damageTypeName;


        public InternationalString DamageTypeName => _damageTypeName;


        public override bool Equals(object obj)
        {
            return obj is DamageTypeObject @object &&
                   base.Equals(obj) &&
                   UniqueID == @object.UniqueID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), UniqueID);
        }
    }
}
