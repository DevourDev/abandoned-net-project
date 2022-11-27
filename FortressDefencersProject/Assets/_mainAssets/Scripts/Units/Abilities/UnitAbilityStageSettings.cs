using DevourDev.MonoBase.AbilitiesSystem;
using FD.Units.Ai;
using FD.Units.Stats;
using UnityEngine;

namespace FD.Units.Abilities
{
    [System.Serializable]
    public class UnitAbilityStageSettings : AbilityStageSettings<DynamicStatObject, UnitAi, ConditionalUnitActions>
    {
        [SerializeField] private Ai.Actions.UnitAction[] _clientSideActions;

        public Ai.Actions.UnitAction[] ClientSideActions => _clientSideActions;
    }
}
