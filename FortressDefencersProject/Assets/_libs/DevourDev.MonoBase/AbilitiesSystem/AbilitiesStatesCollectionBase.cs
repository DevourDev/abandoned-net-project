using DevourDev.Base;
using DevourDev.MonoBase.Ai.ExExAct;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase.AbilitiesSystem
{
    public abstract class AbilitiesStatesCollectionBase<AState, AObj, DynamicStatFamily, AgentAi, AgentDynamicStatsCollection, ConditionalActions, AbilityStageSettingsT>
         where AgentAi : AiBase<AgentAi, ConditionalActions>
        where DynamicStatFamily : ScriptableObject
        where AgentDynamicStatsCollection : DynamicStatsCollectionBase<DynamicStatFamily>
        where AState : AbilityState<DynamicStatFamily, AgentAi, AState, AgentDynamicStatsCollection, ConditionalActions, AObj, AbilityStageSettingsT>
        where AObj : AbilityObjectBase<DynamicStatFamily, AgentAi, AState, AgentDynamicStatsCollection, ConditionalActions, AObj, AbilityStageSettingsT>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
        where AbilityStageSettingsT : AbilityStageSettings<DynamicStatFamily, AgentAi, ConditionalActions>

    {
        private readonly Dictionary<int, AState> _collection;


        public AbilitiesStatesCollectionBase()
        {
            _collection = new();
        }


        public Dictionary<int, AState> Collection => _collection;


        public void Evaluate(AgentAi ai, AgentDynamicStatsCollection agentDynamicStatsCollection)
        {
            foreach (var item in _collection)
            {
                item.Value.Evaluate(ai, agentDynamicStatsCollection);
            }
        }

    }
}