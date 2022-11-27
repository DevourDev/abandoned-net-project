using DevourDev.MonoBase.Ai.ExExAct;

namespace FD.Units.Ai.Enquirers
{
    public abstract class UnitEnquirer : EnquirerBase<UnitAi, ConditionalUnitActions>
    {
        protected Abilities.UnitAbilityState GetAbilityState(UnitAi ai, Abilities.UnitAbilityObject ab)
        {
            return ai.ServerSideUnit.AbilitiesCollection.Collection[ab.UniqueID];
        }
    }
}
