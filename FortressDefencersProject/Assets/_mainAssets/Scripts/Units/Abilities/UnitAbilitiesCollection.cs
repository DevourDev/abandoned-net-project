using DevourDev.MonoBase.AbilitiesSystem;
using FD.Units.Ai;
using FD.Units.Stats;

namespace FD.Units.Abilities
{
    public class UnitAbilitiesCollection : AbilitiesStatesCollectionBase
        <UnitAbilityState, UnitAbilityObject, DynamicStatObject, UnitAi, DynamicStatsCollection, ConditionalUnitActions,
        UnitAbilityStageSettings>
    {

        public void AddAbility(UnitAbilityObject abObj)
        {
            Collection.Add(abObj.UniqueID, new(abObj));
        }
    }

  
}
