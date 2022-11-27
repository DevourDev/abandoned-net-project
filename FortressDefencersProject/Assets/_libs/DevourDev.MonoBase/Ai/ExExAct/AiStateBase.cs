using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{

    public abstract class AiStateBase<AgentAi, ConditionalActions> : GameDatabaseElement
        where AgentAi : AiBase<AgentAi,  ConditionalActions>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        [SerializeField] private ConditionalActions[] _enteringServerSideActions;
        [SerializeField] private AiStateTransition<AgentAi, ConditionalActions>[] _stateTransitions;
        [SerializeField] private ConditionalActions[] _stayingServerSideActions;


        public void Enter(AgentAi ai)
        {
            ai.CurrentState = this;
            HandleEntering(ai);
        }

        public void Stay(AgentAi ai)
        {
            HandleStaying(ai);
        }

        protected virtual void HandleEntering(AgentAi ai)
        {
            foreach (var ca in _enteringServerSideActions)
            {
                ca.Evaluate(ai);
            }
        }

        public void Evaluate(AgentAi ai)
        {
            foreach (var trans in _stateTransitions)
            {
                if (trans.CheckForTransition(ai))
                {
                    return;
                }
            }

            HandleStaying(ai);
        }


        protected virtual void HandleStaying(AgentAi ai)
        {
            foreach (var ca in _stayingServerSideActions)
            {
                ca.Evaluate(ai);
            }
        }
    }

}
