using DevourDev.MonoBase.AbilitiesSystem;
using UnityEngine;

namespace FD.Units.Abilities
{
    [CreateAssetMenu(menuName = "FD/Units/Abilities/Agressive/Default Attack")]
    public class DefaultAtttackUnitAbility : UnitAbilityObject
    {
        private void Awake()
        {
            TargetMode = AbilityTargetMode.Agent;
            CooldownStartingStage = AbilityStage.CastStart;
            ReadyFromStart = true;
        }
    }
}
