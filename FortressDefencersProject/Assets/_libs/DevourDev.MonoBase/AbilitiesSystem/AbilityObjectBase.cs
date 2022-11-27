using DevourDev.Base;
using DevourDev.MonoBase.Ai.ExExAct;
using DevourDev.MonoBase.Multilanguage;
using FD.Units.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase.AbilitiesSystem
{
    public abstract class AbilityObjectBase<DynamicStatFamily, AgentAi, AgentAbilitiesState, AgentDynamicStatsCollection, ConditionalActions, AbilityObject, StageSettingsT> : GameDatabaseElement
        where AgentAi : AiBase<AgentAi, ConditionalActions>
        where AgentDynamicStatsCollection : DynamicStatsCollectionBase<DynamicStatFamily>
        where AgentAbilitiesState : AbilityState<DynamicStatFamily, AgentAi, AgentAbilitiesState, AgentDynamicStatsCollection, ConditionalActions, AbilityObject, StageSettingsT>
        where DynamicStatFamily : ScriptableObject
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
        where AbilityObject : AbilityObjectBase<DynamicStatFamily, AgentAi, AgentAbilitiesState, AgentDynamicStatsCollection, ConditionalActions, AbilityObject, StageSettingsT>
        where StageSettingsT : AbilityStageSettings<DynamicStatFamily, AgentAi, ConditionalActions>
    {
        public class StageSettingDicValue
        {
            public StageSettingDicValue(StageSettingsT stageSettings, int orderIndex)
            {
                StageSettings = stageSettings;
                OrderIndex = orderIndex;
            }


            public StageSettingsT StageSettings { get; set; }
            public int OrderIndex { get; set; }
        }


        [SerializeField] private AbilityTargetMode _targetType;
        [Tooltip("For Unit/Area targeted abilities.")]
        [SerializeField] private float _castDistance;
        [SerializeField] private AbilityStage _cooldownStartingStage;
        [SerializeField] private float _cooldown;
        [SerializeField] private bool _readyFromStart;
        [SerializeField] private StageSettingsT[] _stagesSettings;

        [SerializeField] private bool _hasPassiveActions;
        //[SerializeField] private 

        [System.NonSerialized] private Dictionary<AbilityStage, StageSettingDicValue> _stagesSettingsDic;
        [System.NonSerialized] private Dictionary<DynamicStatFamily, DynamicStatAmount<DynamicStatFamily>> _allStagesCost;

        private float? _sqrCastDist;

        public Dictionary<AbilityStage, StageSettingDicValue> StagesSettingsDic
        {
            get
            {
                if (_stagesSettingsDic == null || _stagesSettingsDic.Count != _stagesSettings.Length)
                {
                    _stagesSettingsDic = new(_stagesSettings.Length);

                    for (int i = 0; i < _stagesSettings.Length; i++)
                    {
                        var s = _stagesSettings[i];
                        _stagesSettingsDic.Add(s.Stage, new(s, i));
                    }
                }

                return _stagesSettingsDic;

            }
        }
        public Dictionary<DynamicStatFamily, DynamicStatAmount<DynamicStatFamily>> AllStagesCost
        {
            get
            {
                if (_allStagesCost == null)
                {
                    _allStagesCost = new();
                    foreach (var stageS in _stagesSettings)
                    {
                        foreach (var stageCosts in stageS.Costs)
                        {
                            if (_allStagesCost.TryGetValue(stageCosts.Stat, out var s))
                            {
                                s.Amount += stageCosts.Amount;
                            }
                            else
                            {
                                _allStagesCost.Add(stageCosts.Stat, new(stageCosts.Stat, stageCosts.Amount));
                            }
                        }
                    }
                }

                return _allStagesCost;
            }
        }

        public StageSettingsT[] StagesSettingsArr { get => _stagesSettings; protected set => _stagesSettings = value; }

        public AbilityTargetMode TargetMode { get => _targetType; protected set => _targetType = value; }
        public float CastDistance { get => _castDistance; protected set => _castDistance = value; }
        public float SqrCastDistance
        {
            get
            {
                if (!_sqrCastDist.HasValue)
                {
                    _sqrCastDist = CastDistance * CastDistance;
                }

                return _sqrCastDist.Value;
            }
        }
        public AbilityStage CooldownStartingStage { get => _cooldownStartingStage; protected set => _cooldownStartingStage = value; }
        public float Cooldown { get => _cooldown; protected set => _cooldown = value; }
        public bool ReadyFromStart { get => _readyFromStart; protected set => _readyFromStart = value; }


        public virtual bool CheckCoolDown(AgentAbilitiesState state)
        {
            return state.IsReady;
        }

        public virtual bool CheckAllStagesCosts(DynamicStatsCollectionBase<DynamicStatFamily> collection)
        {
            foreach (var c in AllStagesCost)
            {
                if (!collection.CanSpend(c.Value))
                {
                    return false;
                }
            }

            return true;
        }
        //TODO later: add out parameters of lacking DynamicStatsAmounts and do full check cycle
        public virtual bool CheckCosts(AbilityStage stage, DynamicStatsCollectionBase<DynamicStatFamily> collection)
        {
            if (_stagesSettingsDic.TryGetValue(stage, out var settings))
            {
                foreach (var cost in settings.StageSettings.Costs)
                {
                    if (!collection.CanSpend(cost))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                Debug.LogError("Unexpected stage check requested!");
                throw new System.ArgumentException();
            }
        }

        public virtual void SpendCosts(AbilityStage stage, DynamicStatsCollectionBase<DynamicStatFamily> collection)
        {
            if (_stagesSettingsDic.TryGetValue(stage, out var settings))
            {
                foreach (DynamicStatAmountObject<DynamicStatFamily> cost in settings.StageSettings.Costs)
                {
                    if (!collection.TrySpend(cost))
                    {
                        Debug.LogError("Not enough resourses to spend!");
                        throw new System.ArgumentException();
                    }
                }
            }
            else
            {
                Debug.LogError("Unexpected stage (SpendCosts): " + stage.ToString());
                throw new System.ArgumentException();
            }
        }
        public virtual void Use(AgentAbilitiesState states)
        {
            if (_stagesSettings == null || _stagesSettings.Length == 0)
            {

                Debug.LogError("Zero stages!");
                return;
            }
            var startStage = _stagesSettings[0].Stage;

            states.SetStage(startStage);
        }

    }
}