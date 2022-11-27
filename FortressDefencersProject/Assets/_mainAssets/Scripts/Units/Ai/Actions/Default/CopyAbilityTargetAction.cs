using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Default/Copy Ability_A Target to Ability_B Target")]
    public class CopyAbilityTargetAction : UnitAction
    {
        [SerializeField] private Abilities.UnitAbilityObject _copyFrom;
        [SerializeField] private Abilities.UnitAbilityObject _copyTo;

        public override void Act(UnitAi ai)
        {
            var abilities = ai.ServerSideUnit.AbilitiesCollection.Collection;
            var ct = abilities[_copyTo.UniqueID];
            var cf = abilities[_copyFrom.UniqueID];
            ct.Target.Set(cf.Target);
        }
    }
}
