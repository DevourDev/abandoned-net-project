using FD.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Global.Rules
{
    public abstract class UnitsInRangeRuleBase : ScriptableObject
    {
        public abstract void GetUnitsInRange(Vector3 origin, float radius, List<UnitOnSceneBase> collection, UnitOnSceneBase ignoringUnit);
        public abstract void GetUnitsInRange(Vector3 origin, float radius, List<UnitOnSceneBase> collection, params UnitOnSceneBase[] ignoringUnits);
        public abstract void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, HashSet<UnitOnSceneBase> ignoringUnits);
        public abstract void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, UnitAllyMode allyMode, UnitOnSceneBase allyModeRelatedTo, UnitOnSceneBase ignoringUnit);
        public abstract void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, UnitAllyMode allyMode, UnitOnSceneBase allyModeRelatedTo, HashSet<UnitOnSceneBase> ignoringUnits);
    }
}
