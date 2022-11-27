using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    public abstract class AgentActionBase<AgentAi, ConditionalActions> : ScriptableObject
        where AgentAi : AiBase<AgentAi, ConditionalActions>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        public abstract void Act(AgentAi ai);
    }

}
