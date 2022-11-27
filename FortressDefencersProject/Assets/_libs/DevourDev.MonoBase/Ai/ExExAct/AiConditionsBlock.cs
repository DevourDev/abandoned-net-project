using System;
using UnityEngine;

namespace DevourDev.MonoBase.Ai.ExExAct
{
    [Serializable]
    public class AiConditionsBlock<AgentAi, ConditionalActions>
            where AgentAi : AiBase<AgentAi, ConditionalActions>
            where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        [SerializeField] private CheckingConditionsMode _checkingMode;
        [SerializeField] private EnquirerBase<AgentAi, ConditionalActions>[] _conditions;


        public bool GetResult(AgentAi ai)
        {
            bool succeed = _checkingMode == CheckingConditionsMode.All;

            if (_conditions != null && _conditions.Length > 0)
            {
                foreach (var c in _conditions)
                {
                    var enquiryResult = ai.GetEnquirerData(c, true).EnquiryResult;
                    switch (_checkingMode)
                    {
                        case CheckingConditionsMode.Any:
                            if (enquiryResult)
                            {
                                return true;
                            }
                            break;
                        case CheckingConditionsMode.All:
                            if (!enquiryResult)
                            {
                                return false;
                            }
                            break;
                        default:

                            Debug.LogError("Unexpected CheckingConditionsMode");
                            throw new NotImplementedException();
                    }
                }
            }

            return succeed;
        }
    }

}
