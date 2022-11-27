using System.Collections.Generic;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    public static class Enquiries
    {
        //public static bool TryGetFirstAliver(List<UnitOnSceneBase> units, out UnitOnSceneBase firstAliver)
        //{
        //    foreach (var u in units)
        //    {
        //        if (u.Alive)
        //        {
        //            firstAliver = u;
        //            return true;
        //        }
        //    }

        //    firstAliver = null;
        //    return false;
        //}

        public static bool CheckAlly(UnitAllyMode allyMode, UnitOnSceneBase uA, UnitOnSceneBase uB)
        {
            var ownerA = uA.Owner;
            var ownerB = uB.Owner;

            return allyMode switch
            {
                UnitAllyMode.OnePlayer => ownerA.UniqueID == ownerB.UniqueID,
                UnitAllyMode.OneTeam => ownerA.TeamID == ownerB.TeamID,
                UnitAllyMode.Enemy => ownerA.TeamID != ownerB.TeamID,
                _ => true,
            };
        }

        /// <summary>
        /// rename me
        /// </summary>
        /// <param name="allyMode"></param>
        /// <returns></returns>
        public static void GetUnitsOfAllyMode(UnitAllyMode allyMode, List<UnitOnSceneBase> allUnits, UnitOnSceneBase relative, List<UnitOnSceneBase> sortedUnits)
        {
            if (allUnits == null)
                throw new System.ArgumentNullException();

            if (sortedUnits == null)
                throw new System.ArgumentNullException();

            sortedUnits.Clear();

            foreach (var uu in allUnits)
            {
                if (CheckAlly(allyMode, uu, relative))
                    sortedUnits.Add(uu);
            }
        }
        public static (UnitOnSceneBase closestUnit, float sqrDist) GetClosestUnitTo(Vector3 origin, List<UnitOnSceneBase> units)
        {
            if (units.Count == 0)
                throw new System.Exception("Zero-lengthed collection can not be processed");

            var closest = units[0];
            var closestD = CalcD(closest);

            for (int i = 1; i < units.Count; i++)
            {
                var d = CalcD(units[i]);
                if (d < closestD)
                {
                    closest = units[i];
                    closestD = d;
                }
            }

            return (closest, closestD);


            float CalcD(UnitOnSceneBase u)
            {
                return (origin - u.transform.position).sqrMagnitude;
            }
        }

        //public static bool TryGetAlivers(List<UnitOnSceneBase> units, out List<UnitOnSceneBase> alivers, int startCapacity = 32)
        //{
        //    alivers = new(32);
        //    var aliversCount = GetAliversNonAllocate(units, alivers);

        //    return aliversCount > 0;
        //}

        //public static int GetAliversNonAllocate(List<UnitOnSceneBase> units, List<UnitOnSceneBase> alivers)
        //{
        //    alivers.Clear();

        //    foreach (var u in units)
        //    {
        //        if (u.Alive)
        //            alivers.Add(u);
        //    }

        //    return alivers.Count;
        //}
    }
}
