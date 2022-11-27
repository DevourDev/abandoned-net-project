using DevourDev.Base;
using FD.Units.Stats;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Set Ability Unit Target")]
    public class SetAbilityUnitTargetAction : SetAbilityTargetActionBase
    {
        [SerializeField] private UnitAllyMode _allyMode;
        [SerializeField] private int _allyModePriority;
        [SerializeField] private bool _allyModeAbsolutePriority;

        [SerializeField] private DistanceMode _rangeMode;
        [SerializeField] private int _rangeModePriority;
        [SerializeField] private bool _rangeModeAbsolutePriority;

        [SerializeField] private LowestHighestEnum _dynamicStatFullnessMode;
        [SerializeField] private DynamicStatObject _dynamicStatObject;
        [SerializeField] private int _dynamicStatFullnessModePriority;
        [SerializeField] private bool _dynamicStatFullnessAbsolutePriority;


        public override void Act(UnitAi ai)
        {
            throw new System.NotImplementedException();
            // Информация берётся из GameManager.AllActiveUnits, для которой нужны PlayerResources, TeamObject и т.д.
            UnitOnSceneBase bestTarget;

            if (_allyMode != UnitAllyMode.None)
            {

            }

            if (_rangeMode != DistanceMode.None)
            {

            }

            if(_dynamicStatFullnessMode != LowestHighestEnum.None)
            {

            }
        }
    }

}
