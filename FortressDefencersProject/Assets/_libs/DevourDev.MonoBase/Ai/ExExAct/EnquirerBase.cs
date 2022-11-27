using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    /// <summary>
    /// Проверяет что-то и сохраняет результат проверки в EnquirerData.
    /// </summary>
    /// <typeparam name="AgentAi"></typeparam>
    public abstract class EnquirerBase<AgentAi, ConditionalActions> : ScriptableObject
        where AgentAi : AiBase<AgentAi, ConditionalActions>
         where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        public void Examine(AgentAi ai)
        {
            Examine(ai, out var result, out var ed);
            ed.EnquiryResult = result;
            ed.SetActual();
        }
        protected abstract void Examine(AgentAi ai, out bool result, out EnquirerData ed);
    }

}
