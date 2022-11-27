using System;
using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    [Serializable]
    public class AiStateTransition<AgentAi, ConditionalActions>
        where AgentAi : AiBase<AgentAi, ConditionalActions>
         where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        [SerializeField] private AiConditionsBlock<AgentAi, ConditionalActions> _conditionsBlock;
        [SerializeField] private AiStateBase<AgentAi, ConditionalActions> _succeedState;
        [SerializeField] private bool _hasFailureState;
        [SerializeField] private AiStateBase<AgentAi, ConditionalActions> _failureState;


        public bool CheckForTransition(AgentAi ai)
        {
            if (_conditionsBlock.GetResult(ai))
            {
                _succeedState.Enter(ai);
                return true;
            }

            if (_hasFailureState)
            {
                _failureState.Enter(ai);
                return true;
            }


            return false;
        }
    }

}
