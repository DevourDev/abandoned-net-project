using DevourDev.Base;
using DevourDev.MonoBase.Ai.ExExAct;
using UnityEngine;

namespace DevourDev.MonoBase.AbilitiesSystem
{
    [System.Serializable]
    public class AbilityStageSettings<DynamicStatFamily, AgentAi, ConditionalActions>
     where AgentAi : AiBase<AgentAi, ConditionalActions>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
     where DynamicStatFamily : ScriptableObject
    {
        [SerializeField] private AbilityStage _stage;
        [SerializeField] private float _duration;
        [Tooltip("If duration of being in this stage cannot be predefined")]
        [SerializeField] private bool _nonConstantDuration;
        [SerializeField] private DynamicStatAmountObject<DynamicStatFamily>[] _costs;
        [SerializeField] private AgentActionBase<AgentAi, ConditionalActions>[] _serverSideActions;


        public AbilityStage Stage => _stage;
        public float Duration => _duration;
        public bool UnlimitedDuration => _nonConstantDuration;
        public DynamicStatAmountObject<DynamicStatFamily>[] Costs => _costs;
        public AgentActionBase<AgentAi, ConditionalActions>[] ServerSideActions => _serverSideActions;
    }

}