using DevourDev.MonoBase.AbilitiesSystem;
using FD.Units.Abilities;
using FD.Units.Ai;
using FD.Units.Stats;

namespace FD.Units.Abilities
{
    public class UnitAbilityState : AbilityState<DynamicStatObject, UnitAi, UnitAbilityState, DynamicStatsCollection, ConditionalUnitActions, UnitAbilityObject, UnitAbilityStageSettings>
    {
        private readonly Target _target;


        public UnitAbilityState(UnitAbilityObject reference) : base(reference)
        {
            _target = new();
        }


        public Target Target => _target;

        protected override void HandleStage(UnitAi ai)
        {
            var nm = FD.Networking.NetworkManager.Instance;

            if (nm.Mode == DevourDev.Networking.NetworkMode.Server || nm.Mode == DevourDev.Networking.NetworkMode.Host)
            {
                var gm = Global.GameManager.Instance;

                gm.RegistrateAbilityStage(ai, this); //key exists exception (fix is not needed: exception only occures from other (upper) exception)

                foreach (var ssa in Reference.StagesSettingsDic[CurrentStage].StageSettings.ServerSideActions)
                {
                    ssa.Act(ai);
                }
            }
            if (nm.Mode == DevourDev.Networking.NetworkMode.Client || nm.Mode == DevourDev.Networking.NetworkMode.Host)
            {
                foreach (var csa in Reference.StagesSettingsDic[CurrentStage].StageSettings.ClientSideActions)
                {
                    csa.Act(ai);
                }
            }
        }
    }
}