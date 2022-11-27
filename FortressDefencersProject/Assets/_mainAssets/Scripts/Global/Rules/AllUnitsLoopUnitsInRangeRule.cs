using FD.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FD.Global.Rules
{
    //[CreateAssetMenu(menuName = "FD/Game Rules/Units in range/All Units Loop"), System.Obsolete("Not implemented yet")] (implemented partically)
    public class AllUnitsLoopUnitsInRangeRule : UnitsInRangeRuleBase
    {
        private GameManager _gm;
        private GameRulesObject _grules;


        protected GameManager GM
        {
            get
            {
                if (_gm == null)
                {
                    _gm = GameManager.Instance;
                }

                return _gm;
            }
        }

        protected GameRulesObject GameRules
        {
            get
            {
                if (_grules == null)
                {
                    _grules = GM.GameRules;
                }

                return _grules;
            }
        }


        public override void GetUnitsInRange(Vector3 origin, float radius, List<UnitOnSceneBase> collection, params UnitOnSceneBase[] ignoringUnits)
        {
            float sqrRadius = radius * radius;

            collection.Clear();

            foreach (var side in GM.AllActiveUnits)
            {
                foreach (var u in side.Value)
                {
                    var p = u.Value.transform.position;
                    var sqrDist = (origin - p).sqrMagnitude;

                    if (sqrDist <= sqrRadius && !ignoringUnits.Contains(u.Value))
                        collection.Add(u.Value);
                }
            }
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, List<UnitOnSceneBase> collection, UnitOnSceneBase ignoringUnit)
        {
            float sqrRadius = radius * radius;

            collection.Clear();

            foreach (var side in GM.AllActiveUnits)
            {
                foreach (var u in side.Value)
                {
                    var p = u.Value.transform.position;
                    var sqrDist = (origin - p).sqrMagnitude;

                    if (sqrDist <= sqrRadius && ignoringUnit != u.Value)
                        collection.Add(u.Value);
                }
            }
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, HashSet<UnitOnSceneBase> ignoringUnits)
        {
            throw new System.NotImplementedException();
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, UnitAllyMode allyMode, UnitOnSceneBase allyModeRelatedTo, UnitOnSceneBase ignoringUnit)
        {
            float sqrRadius = radius * radius;

            collection.Clear();

            foreach (var side in GM.AllActiveUnits)
            {
                foreach (var u in side.Value)
                {
                    var p = u.Value.transform.position;
                    var sqrDist = (origin - p).sqrMagnitude;

                    if (sqrDist <= sqrRadius && Units.Ai.Enquirers.Enquiries.CheckAlly(allyMode, allyModeRelatedTo, u.Value) && ignoringUnit != u.Value)
                        collection.Add(u.Value);
                }
            }
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, UnitAllyMode allyMode, UnitOnSceneBase allyModeRelatedTo, HashSet<UnitOnSceneBase> ignoringUnits)
        {
            throw new System.NotImplementedException();
        }
    }
}
