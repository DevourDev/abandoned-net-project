using DevourDev.Base;
using DevourDev.MonoBase.Ai.ExExAct;
using FD.Global;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase.AbilitiesSystem
{
    public abstract class AbilityState<DynamicStatFamily, AgentAi, AgentAbilitiesStates, AgentDynamicStatsCollection, ConditionalActions, AbilityObject, StageSettingsT>
         where AgentAi : AiBase<AgentAi, ConditionalActions>
        where AgentDynamicStatsCollection : DynamicStatsCollectionBase<DynamicStatFamily>
        where DynamicStatFamily : ScriptableObject
        where AgentAbilitiesStates : AbilityState<DynamicStatFamily, AgentAi, AgentAbilitiesStates, AgentDynamicStatsCollection, ConditionalActions, AbilityObject, StageSettingsT>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
        where AbilityObject : AbilityObjectBase<DynamicStatFamily, AgentAi, AgentAbilitiesStates, AgentDynamicStatsCollection, ConditionalActions, AbilityObject, StageSettingsT>
        where StageSettingsT : AbilityStageSettings<DynamicStatFamily, AgentAi, ConditionalActions>

    {
        private readonly AbilityObject _reference;
        private readonly Dictionary<AbilityStage, StageState> _stagesStates;
        private AbilityStage _currentStage;


        private StageState CurrentStageState => _stagesStates[_currentStage];


        public AbilityState(AbilityObject reference)
        {
            _reference = reference;
            _currentStage = AbilityStage.None;
            _stagesStates = new();

            _stagesStates.Add(AbilityStage.None, new());

            foreach (var settings in _reference.StagesSettingsDic)
            {
                _stagesStates.Add(settings.Value.StageSettings.Stage, new());
            }

        }


        public double ReadyTime
        {
            get
            {
                if (_stagesStates.TryGetValue(_reference.CooldownStartingStage, out var v))
                {
                    return v.EnteredTime + _reference.Cooldown;
                }
                else
                {
                    return Time.timeAsDouble;
                }
            }
        }

        public bool IsReady => Time.timeAsDouble >= ReadyTime;

        public AbilityObject Reference => _reference;
        public AbilityStage CurrentStage => _currentStage;


        public void SetStage(AbilityStage stage)
        {
            CurrentStageState.EndStage();
            _currentStage = stage;
        }

        public void Evaluate(AgentAi ai, AgentDynamicStatsCollection dynamicStatsCollection)
        {
            if (_currentStage == AbilityStage.None)
                return;

            if (!_reference.CheckCosts(CurrentStage, dynamicStatsCollection))
            {
                Debug.Log("НЕ ХВАТАЕТ БЛЯДЬ!!!!");
                CurrentStageState.EndStage();
                _currentStage = AbilityStage.None;
                return;
            }

            if (!CurrentStageState.SessionIsActive)
                CurrentStageState.StartStage();

            SpendCosts(dynamicStatsCollection);

            HandleStage(ai);

            CheckForStageTransition(dynamicStatsCollection);
        }

        private void SpendCosts(AgentDynamicStatsCollection dynamicStatsCollection)
        {
            _reference.SpendCosts(CurrentStage, dynamicStatsCollection);
        }

        protected virtual void HandleStage(AgentAi ai)
        {
            foreach (var ssa in _reference.StagesSettingsDic[_currentStage].StageSettings.ServerSideActions)
            {
                ssa.Act(ai);
            }
        }

        private bool CheckForStageTransition(AgentDynamicStatsCollection dynamicStatsCollection)
        {
            bool stageFinished;

            if (_reference.StagesSettingsDic[_currentStage].StageSettings.UnlimitedDuration)
            {
                stageFinished = !_reference.CheckCosts(_currentStage, dynamicStatsCollection);
            }
            else
            {
                stageFinished = CurrentStageState.EnteredTime + _reference.StagesSettingsDic[_currentStage].StageSettings.Duration <= Time.timeAsDouble;
                if (stageFinished)
                {
                    CurrentStageState.EndStage();
                    _currentStage = GetNextAbilityStage();
                }
            }

            return stageFinished;
        }

        private AbilityStage GetNextAbilityStage()
        {
            int nextStageIndex = _reference.StagesSettingsDic[_currentStage].OrderIndex + 1;
            if (nextStageIndex == _reference.StagesSettingsArr.Length)
            {
                return AbilityStage.None;
            }

            //return (AbilityStage)nextStageIndex;
            var nextStage = _reference.StagesSettingsArr[nextStageIndex].Stage;
            return nextStage;
        }


        private class StageState
        {
            public bool SessionIsActive { get; private set; }
            public double EnteredTime { get; private set; }

            public Dictionary<AgentActionBase<AgentAi, ConditionalActions>, AgentActionData> ActionsData { get; set; }

            public StageState()
            {
                SessionIsActive = false;
                EnteredTime = 0;
            }


            public void StartStage()
            {
                SessionIsActive = true;
                EnteredTime = Time.timeAsDouble;
            }

            public void EndStage()
            {
                SessionIsActive = false;
            }


            public bool ContainsActionData(AgentActionBase<AgentAi, ConditionalActions> action)
            {
                return ActionsData.ContainsKey(action);
            }

            public T GetActionData<T>(AgentActionBase<AgentAi, ConditionalActions> action) where T : AgentActionData
            {
                return (T)ActionsData[action];
            }

            public T GetOrCreateActionData<T>(AgentActionBase<AgentAi, ConditionalActions> action) where T : AgentActionData, new()
            {
                if (ContainsActionData(action))
                {
                    return GetActionData<T>(action);
                }

                var d = new T();
                SetActionData(action, d);
                return d;
            }

            public void SetActionData(AgentActionBase<AgentAi, ConditionalActions> action, AgentActionData ad)
            {
                ActionsData.Add(action, ad);
            }
        }

    }
}