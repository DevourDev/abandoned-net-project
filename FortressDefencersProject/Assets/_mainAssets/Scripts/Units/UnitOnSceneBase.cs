using DevourDev.Base;
using FD.Global.Sides;
using FD.Units.Abilities;
using FD.Units.Ai;
using FD.Units.Ai.Sensors;
using FD.Units.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FD.ClientSide.Units
{
}

namespace FD.Units
{

    public class UnitOnSceneBase : MonoBehaviour, IUnique
    {
        public event System.Action<UnitOnSceneBase> OnDeath;

        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private Vector3 _spawnPoint;
        private UnitObject _reference;
        private GameSideDefault _owner;
        private UnitAi _ai;
        private UnitAbilitiesCollection _abilitiesCollection;
        private RealUnitStats _realStats;


        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public Vector3 SpawnPoint => _spawnPoint;
        public UnitObject Reference => _reference;
        public GameSideDefault Owner => _owner;
        public RealUnitStats RealStats => _realStats;
        public UnitAi Ai => _ai;
        public DynamicStatsCollection DynamicStatsCollection => _realStats.Common.DynamicStats;
        public UnitAbilitiesCollection AbilitiesCollection => _abilitiesCollection;

        //public bool Alive { get; set; }
        public UnitOnSceneBase Lasthitter { get; set; }
        public int UniqueID { get; set; }



        private void Awake()
        {
        }


        public void Init(UnitObject reference, GameSideDefault owner)
        {
            _spawnPoint = transform.position;
            _reference = reference;
            SetOwner(owner);
            InitRealStats();
            InitAi();
            InitAbilities();
            SetNavMeshProps();

            OnDeath += OnDeathHandler;
        }
        private void InitAi()
        {
            _ai = new(this);
            Ai.CurrentState = Reference.DefaultState;
        }
        private void InitAbilities()
        {
            _abilitiesCollection = new();
            foreach (var ab in Reference.Abilities)
            {
                _abilitiesCollection.AddAbility(ab);
            }
        }
        private void SetOwner(GameSideDefault side)
        {
            _owner = side;
        }
        private void InitRealStats()
        {
            _realStats = new();
            var realCS = RealStats.Common;
            var refCS = Reference.CommonStats;
            realCS.Acceleration = refCS.Acceleration;

            InitDynamicStats();

            realCS.Acceleration = refCS.Acceleration;
            realCS.LethalDynamicStat = refCS.LethalDynamicStat;
            realCS.MoveSpeed = refCS.MoveSpeed;
            realCS.RotationSpeed = refCS.RotationSpeed;
            realCS.VisionRange = refCS.VisionRange;
        }

        private void InitDynamicStats()
        {
            var realCS = RealStats.Common;
            var refCS = Reference.CommonStats;
            realCS.DynamicStats = new();
            foreach (var refDS in refCS.DynamicStats)
            {
                refDS.CreateDynamicStat(realCS.DynamicStats);
            }

            foreach (var ds in realCS.DynamicStats.Stats)
            {
                Global.GameManager.Instance.Ticker.OnTick += (ticker, tps) => ds.Value.Regenerate(tps); // плохо
            }
        }

        private void SetNavMeshProps()
        {
            NavMeshAgent.speed = RealStats.Common.MoveSpeed;
            NavMeshAgent.angularSpeed = RealStats.Common.RotationSpeed;
            NavMeshAgent.acceleration = RealStats.Common.Acceleration;

            NavMeshAgent.stoppingDistance = NavMeshAgent.baseOffset * 1.2f;
        }

        public void Die()
        {
            OnDeath?.Invoke(this);
        }

        private void OnDeathHandler(UnitOnSceneBase obj)
        {

        }

        [SerializeField] private TMPro.TextMeshProUGUI _currentStateText;

        private void Update()
        {
            //return;
            if (Ai == null || Ai.CurrentState == null)
                return;

            _currentStateText.text = Ai.CurrentState.name;
        }

        public void MakeTurn()
        {
            Ai.Evaluate();
            AbilitiesCollection.Evaluate(Ai, DynamicStatsCollection);
            Ai.SetOutdatedAllData();


        }

        public void TakeDamage(DamageTypeObject type, float sourceAmount, UnitOnSceneBase dmgDealer)
        {

            //Debug.Log("taking dmg...");
            Lasthitter = dmgDealer;
            var grm = Global.GameManager.Instance.GameRules;

            float finalDmg = grm.DamageCalculationRule.CalculateFinalDamage(type, sourceAmount, _reference.BattleStats.Defensive.ArmorType, _reference.BattleStats.Defensive.ArmorValue);

            //Debug.Log("final dmg == " + finalDmg);
            var lethalStat = _reference.CommonStats.LethalDynamicStat;

            var dsa = new DynamicStatAmount<DynamicStatObject>(lethalStat, finalDmg);

            if (!DynamicStatsCollection.TryGetDynamicStat(lethalStat, out var dstat))
            {
                Debug.LogError("no lethal stat, bro, wtf???");
            }

            bool lethalDmg = !dstat.CanSpend(finalDmg);
            dstat.SpendValueOrAll(finalDmg);

            if (lethalDmg)
            {
                OnDeath?.Invoke(this);
            }
        }


        //------Visual Debugging---------

        [SerializeField] private UnitState _debug_initialState;

        private void OnDrawGizmos()
        {
            return;
            if (!Application.isPlaying)
                return;
            if (!AbilitiesCollection.Collection.ContainsKey(1))
                return;

            if (AbilitiesCollection.Collection[1].Target.TryGetPoint(this, out var p))
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, p);
                //Color c = Color.yellow;
                //c.a = 0.5f;
                //Gizmos.color = c;
                //Gizmos.DrawSphere(p, 2);
            }


        }

        public override bool Equals(object obj)
        {
            return obj is UnitOnSceneBase @base &&
                   base.Equals(obj) &&
                   UniqueID == @base.UniqueID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), UniqueID);
        }
    }

}
