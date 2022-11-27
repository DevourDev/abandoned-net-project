using System.Linq;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Closest Enemy Fortress as Ability Target")]
    public class ClosestEnemyFortressAsAbilityTargetAction : SetAbilityTargetActionBase
    {
        [SerializeField] private SetAbilityTargetActionBase _noFortressAction;
        public override void Act(UnitAi ai)
        {
            if (TryFindFortress(ai, out var f))
            {
                SetTarget(ai, f);
            }
            else
            {
                if (_noFortressAction != null)
                {
                    _noFortressAction.Act(ai);
                }
                else
                {
                    SetTarget(ai, Vector3.zero);
                }
            }
        }

        private bool TryFindFortress(UnitAi ai, out UnitOnSceneBase closestFortress)
        {
            var gm = Global.GameManager.Instance;

            closestFortress = null /*= gm.Sides.First().Value.Fortress*/;
            foreach (var side in gm.Sides)
            {
                if (side.Key == ai.ServerSideUnit.Owner.UniqueID)
                    continue;

                closestFortress = side.Value.Fortress;
            }

            if (closestFortress == null)
            {
                return false;
            }
            float closestDistance = CalcD(closestFortress);

            foreach (var side in gm.Sides)
            {
                if (side.Key == ai.ServerSideUnit.Owner.UniqueID)
                    continue;

                float d = CalcD(side.Value.Fortress);
                if (d < closestDistance)
                {
                    closestFortress = side.Value.Fortress;
                    closestDistance = d;
                }
            }

            return closestFortress;


            float CalcD(UnitOnSceneBase u)
            {
                return (ai.ServerSideUnit.transform.position - u.transform.position).sqrMagnitude;
            }
        }
    }

}
