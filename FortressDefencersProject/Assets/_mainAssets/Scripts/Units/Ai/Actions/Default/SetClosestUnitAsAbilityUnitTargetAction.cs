using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Closest Unit as Ability Unit Target")]
    public class SetClosestUnitAsAbilityUnitTargetAction : SetAbilityTargetActionBase
    {
        [SerializeField] private Enquirers.SortedUnitsInVisionRangeEnquirer _sortedUnitsEnquirer;

        public override void Act(UnitAi ai)
        {
            var units = ai.GetEnquirerData<Enquirers.SortedUnitsInVisionRangeEnquirer.Data>(_sortedUnitsEnquirer, true).SortedUnits;


            if (units.Count == 0)
            {
                Debug.LogError("No Units. Should check with Enquirer first.");
                throw new System.NullReferenceException();
            }

            UnitOnSceneBase closestU = units[0];
            float closestD = CalcD(closestU);

            for (int i = 1; i < units.Count; i++)
            {
                var u = units[i];
                
                float d = CalcD(units[i]);
                if (d < closestD)
                {
                    closestU = units[i];
                    closestD = d;
                }
            }

            SetTarget(ai, closestU);


            float CalcD(UnitOnSceneBase u)
            {
                return (ai.ServerSideUnit.transform.position - u.transform.position).sqrMagnitude;
            }
        }
    }

}
