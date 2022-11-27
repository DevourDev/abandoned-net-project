using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    [System.Serializable]
    public abstract class ConditionalAgentActions<AgentAi, ConditionalActions>
        where AgentAi : AiBase<AgentAi, ConditionalActions>
         where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        [SerializeField] private AiConditionsBlock<AgentAi, ConditionalActions> _conditionsBlock;
        [SerializeField] private AgentActionBase<AgentAi, ConditionalActions>[] _succeedActions;
        [SerializeField] private bool _hasFailureActions;
        [SerializeField] private AgentActionBase<AgentAi, ConditionalActions>[] _failureActions;


        public virtual void Evaluate(AgentAi ai)
        {
            if (_conditionsBlock.GetResult(ai))
            {
                HandleSucceed(ai);
                return;
            }

            if (_hasFailureActions)
            {
                HandleFailure(ai);
                return;
            }
        }

        protected virtual void HandleSucceed(AgentAi ai)
        {
            foreach (var a in _succeedActions)
            {
                a.Act(ai);
            }
        }

        protected virtual void HandleFailure(AgentAi ai)
        {
            foreach (var a in _failureActions)
            {
                a.Act(ai);
            }
        }
    }

}
