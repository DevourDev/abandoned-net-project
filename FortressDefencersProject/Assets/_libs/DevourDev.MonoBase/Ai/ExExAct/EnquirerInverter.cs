using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    public abstract class EnquirerInverter<Enquirer, AgentAi, ConditionalActions> : EnquirerBase<AgentAi, ConditionalActions>
        where AgentAi : AiBase<AgentAi, ConditionalActions>
        where Enquirer : EnquirerBase<AgentAi, ConditionalActions>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        [SerializeField] private Enquirer _enquirer;

        protected override void Examine(AgentAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            result = !ai.GetEnquirerData(_enquirer, true).EnquiryResult;
        }


        public class Data : EnquirerData
        {

        }
    }

}
