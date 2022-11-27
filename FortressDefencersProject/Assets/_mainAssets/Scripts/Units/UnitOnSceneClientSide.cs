using DevourDev.Base;
using FD.Units;
using FD.Units.Abilities;
using FD.Units.Ai;
using FD.Units.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FD.ClientSide.Units
{
    public class UnitSideData
    {
        public int PlayerID { get; set; }
        public int TeamID { get; set; }
    }

    public class UnitOnSceneClientSide : MonoBehaviour, IUnique
    {
        public UnityEngine.Events.UnityEvent DealthEvent;
        [SerializeField] private Animator _animator;
        [SerializeField] private TransformInterpolator _interpolator;

        private UnitSideData _sideData;
        private UnitObject _reference;
        private UnitAi _ai;
        private DynamicStatsCollection _dynamicStatsCollection;
        private UnitAbilitiesCollection _abilitiesCollection;


        public TransformInterpolator Interpolator
        {
            get
            {
                if (_interpolator == null)
                {
                    if (!TryGetComponent<TransformInterpolator>(out var i))
                    {

                        Debug.LogError("INTERPOLATOR NOT FOUND");
                        i = gameObject.AddComponent<TransformInterpolator>();
                    }

                    _interpolator = i;
                }

                return _interpolator;
            }
        }


        public int UniqueID { get; set; }

        public Animator Animator => _animator;
        public UnitSideData SideData => _sideData;
        public UnitObject Reference => _reference;
        public UnitAi Ai => _ai;
        public DynamicStatsCollection DynamicStatsCollection => _dynamicStatsCollection;
        public UnitAbilitiesCollection AbilitiesCollection => _abilitiesCollection;


        public void Init(UnitObject reference, int playerID, int teamID, ICollection<Networking.Realm.GamePackets.DynamicStatAllValuesPacket> statsPackets)
        {
            _reference = reference;

            _sideData = new UnitSideData()
            {
                PlayerID = playerID,
                TeamID = teamID,
            };

            //init RealStats (to fill from Server Messages)
            InitDynamicStats(statsPackets);
            InitAi();
            InitAbilities();
        }

        private void InitDynamicStats(ICollection<Networking.Realm.GamePackets.DynamicStatAllValuesPacket> statsPackets)
        {
            Debug.Log("Initing ds...");
            _dynamicStatsCollection = new(statsPackets.Count);
            foreach (var sP in statsPackets)
            {
                var statObject = ClientSide.Global.ClientSideGameManager.Instance.GameRules.DynamicStatsDatabase.GetElement(sP.StatID);
                if (!_dynamicStatsCollection.Stats.TryAdd(statObject, new DynamicStat(sP.Max, sP.Regen, sP.Min, sP.Current)))
                    throw new Exception($"DynamicStatsCollection already contains stat with ID {sP.StatID}");

                if (!_dynamicStatsCollection.TryGetDynamicStat(statObject, out var s))
                    throw new Exception($"Блять.");
            }
            //foreach (var refDS in Reference.CommonStats.DynamicStats)
            //{
            //    refDS.CreateDynamicStat(_dynamicStatsCollection);
            //}
        }

        private void InitAi()
        {
            _ai = new(this);
        }

        private void InitAbilities()
        {
            _abilitiesCollection = new();
            foreach (var ab in Reference.Abilities)
            {
                _abilitiesCollection.AddAbility(ab);
            }
        }

        public void EvaluateClientSideState(UnitState state, bool enter)
        {
            if (enter)
            {
                state.Enter(Ai);
            }
            else
            {
                state.Stay(Ai);
            }
        }
        public void EvaluateClientSideState(UnitState state)
        {
            if (Ai.CurrentState == state)
            {
                state.Stay(Ai);
            }
            else
            {
                state.Enter(Ai);
            }
        }

        public void EvaluateClientSideAbilityStage(FD.Units.Abilities.UnitAbilityObject abObj, DevourDev.MonoBase.AbilitiesSystem.AbilityStage stage)
        {
            bool succeed = abObj.StagesSettingsDic.TryGetValue(stage, out var settings);

            if (!succeed)
                throw new System.Exception($"Unexpected Ability Stage or Ability Object received from Server. Stage: {stage}, Object: {abObj}");

            AbilitiesCollection.Collection[abObj.UniqueID].SetStage(stage);
            foreach (var a in settings.StageSettings.ClientSideActions)
            {
                a.Act(Ai);
            }
        }

    }
}
