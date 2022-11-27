using FD.Units.Ai;
using UnityEngine;

namespace FD.Global.Rules
{
    [CreateAssetMenu(menuName = "FD/Game Rules/Game Rules Object")]
    public class GameRulesObject : ScriptableObject
    {
        #region Fields
        [SerializeField] private DamageCalculationRuleBase _damageCalculationRule;
        [SerializeField] private UnitsInRangeRuleBase _unitsInRangeRule;
        [SerializeField] private PlayerEliminationHandlerBase _eliminationHandler;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _unitsLayer;

        [SerializeField] private FD.Units.Abilities.AbilitiesDatabaseObject _abilitiesDatabase;
        [SerializeField] private FD.Units.UnitsDatabaseObject _unitsDatabase;
        [SerializeField] private FD.Units.Stats.DynamicStatsDatabaseObject _dynamicStatsDatabase;
        [SerializeField] private UnitStatesDatabaseObject _unitStatesDatabase;
        #endregion


        #region Props
        public DamageCalculationRuleBase DamageCalculationRule => _damageCalculationRule;
        public UnitsInRangeRuleBase UnitsInRangeRule => _unitsInRangeRule;
        public PlayerEliminationHandlerBase EliminationHandler => _eliminationHandler;

        public LayerMask GroundLayer => _groundLayer;
        public LayerMask UnitsLayer => _unitsLayer;

        public FD.Units.Abilities.AbilitiesDatabaseObject AbilitiesDatabase => _abilitiesDatabase;
        public FD.Units.UnitsDatabaseObject UnitsDatabase => _unitsDatabase;
        public FD.Units.Stats.DynamicStatsDatabaseObject DynamicStatsDatabase => _dynamicStatsDatabase;
        public UnitStatesDatabaseObject UnitStatesDatabase => _unitStatesDatabase;

        #endregion
    }
}
