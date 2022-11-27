using FD.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FD.Global.Rules
{
    [CreateAssetMenu(menuName = "FD/Game Rules/Units in range/Overlap Sphere Mode")]
    public class OverlapSphereUnitsInRangeRule : UnitsInRangeRuleBase
    {
        private Collider[] _cols = new Collider[1024 * 8];

        private GameRulesObject _grules;


        private GameRulesObject GRM
        {
            get
            {
                if (_grules == null)
                {
                    _grules = GameManager.Instance.GameRules;
                }

                return _grules;
            }
        }
        private Collider[] Cols
        {
            get
            {
                return _cols;
            }
        }


        public override void GetUnitsInRange(Vector3 origin, float radius, List<UnitOnSceneBase> collection, params UnitOnSceneBase[] ignoringUnits)
        {
            bool nothingToIgnore = ignoringUnits == null || ignoringUnits.Length == 0;
            int colsCount;
            collection.Clear();
            do
            {
                colsCount = Physics.OverlapSphereNonAlloc(origin, radius, Cols, GRM.UnitsLayer);

            } while (!CheckCols(colsCount));

            for (int i = 0; i < colsCount; i++)
            {
                if (Cols[i].TryGetComponent<UnitOnSceneBase>(out var u) && (nothingToIgnore || !ignoringUnits.Contains(u)))
                {
                    collection.Add(u);
                }
            }
        }
        public override void GetUnitsInRange(Vector3 origin, float radius, List<UnitOnSceneBase> collection, UnitOnSceneBase ignoringUnit)
        {
            int colsCount;
            collection.Clear();
            do
            {
                colsCount = Physics.OverlapSphereNonAlloc(origin, radius, Cols, GRM.UnitsLayer);

            } while (!CheckCols(colsCount));

            for (int i = 0; i < colsCount; i++)
            {
                if (Cols[i].TryGetComponent<UnitOnSceneBase>(out var u) && ignoringUnit != u)
                {
                    collection.Add(u);
                }
            }
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, HashSet<UnitOnSceneBase> ignoringUnits)
        {
            bool nothingToIgnore = ignoringUnits == null || ignoringUnits.Count == 0;
            int colsCount;
            collection.Clear();
            do
            {
                colsCount = Physics.OverlapSphereNonAlloc(origin, radius, Cols, GRM.UnitsLayer);

            } while (!CheckCols(colsCount));

            for (int i = 0; i < colsCount; i++)
            {
                if (Cols[i].TryGetComponent<UnitOnSceneBase>(out var u) && (nothingToIgnore || !ignoringUnits.Contains(u)))
                {
                    collection.Add(u);
                }
            }
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, UnitAllyMode allyMode, UnitOnSceneBase allyModeRelatedTo, HashSet<UnitOnSceneBase> ignoringUnits)
        {
            bool nothingToIgnore = ignoringUnits == null || ignoringUnits.Count == 0;
            int colsCount;
            collection.Clear();
            do
            {
                colsCount = Physics.OverlapSphereNonAlloc(origin, radius, Cols, GRM.UnitsLayer);

            } while (!CheckCols(colsCount));

            for (int i = 0; i < colsCount; i++)
            {
                if (Cols[i].TryGetComponent<UnitOnSceneBase>(out var u) && Units.Ai.Enquirers.Enquiries.CheckAlly(allyMode, allyModeRelatedTo, u) && (nothingToIgnore || !ignoringUnits.Contains(u)))
                {
                    collection.Add(u);
                }
            }
        }

        public override void GetUnitsInRange(Vector3 origin, float radius, ICollection<UnitOnSceneBase> collection, UnitAllyMode allyMode, UnitOnSceneBase allyModeRelatedTo, UnitOnSceneBase ignoringUnit)
        {
            int colsCount;
            collection.Clear();
            do
            {
                colsCount = Physics.OverlapSphereNonAlloc(origin, radius, Cols, GRM.UnitsLayer);

            } while (!CheckCols(colsCount));

            for (int i = 0; i < colsCount; i++)
            {
                if (Cols[i].TryGetComponent<UnitOnSceneBase>(out var u) && Units.Ai.Enquirers.Enquiries.CheckAlly(allyMode, allyModeRelatedTo, u) && ignoringUnit != u)
                {
                    collection.Add(u);
                }
            }
        }

        private bool CheckCols(int elementsCount)
        {
            bool result = true;

            if (_cols == null || elementsCount > _cols.Length)
            {
                _cols = new Collider[elementsCount * 2];
                result = false;
            }

            return result;
        }


    }
}
