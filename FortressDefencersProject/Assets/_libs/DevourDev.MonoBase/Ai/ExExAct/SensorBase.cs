using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    public abstract class SensorBase<AgentAi, ConditionalActions> : ScriptableObject
        where AgentAi : AiBase<AgentAi, ConditionalActions>
         where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        public void Scan(AgentAi ai)
        {
            Scan(ai, out var sd);
            sd.SetActual();
        }

        protected abstract void Scan(AgentAi ai, out SensorData sd);
    }

}
