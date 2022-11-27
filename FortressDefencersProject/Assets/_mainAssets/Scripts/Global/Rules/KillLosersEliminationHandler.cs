using FD.Global.Sides;
using UnityEngine;

namespace FD.Global.Rules
{
    [CreateAssetMenu(menuName ="FD/Game Rules/Elimination/Kill losers")]
    public class KillLosersEliminationHandler : PlayerEliminationHandlerBase
    {
        public override void Eliminate(GameSideBase side)
        {
            foreach (var u in side.Resources.ActiveUnits)
            {
                //if (u.Value.Alive/* && u.Value != ((GameSideDefault)side).Fortress*/)
                if (u.Value.UniqueID != ((GameSideDefault)side).Fortress.UniqueID)
                {
                    u.Value.Lasthitter = ((GameSideDefault)side).Fortress.Lasthitter;
                    u.Value.Die();
                }
            }
        }
    }
}
