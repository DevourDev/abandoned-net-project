using DevourDev.Base;
using DevourDev.MonoBase;
using DevourDev.MonoBase.Multilanguage;
using FD.Units.Ai;
using FD.Units.Stats;
using UnityEngine;

namespace FD.Units
{

    [System.Serializable]
    public class BattleStats
    {
        //[SerializeField] private Offence _offensive;
        [SerializeField] private Defence _defensive;


        //public Offence Offensive => _offensive;
        public Defence Defensive => _defensive;


        //[System.Serializable]
        //public class Offence
        //{

        //}


        [System.Serializable]
        public class Defence
        {
            [SerializeField] private float _armorValue;
            [SerializeField] private ArmorTypeObject _armorType;


            public float ArmorValue => _armorValue;
            public ArmorTypeObject ArmorType => _armorType;
        }
    }


    [System.Serializable]
    public class CommonStats
    {
        [SerializeField] private DynamicStatCreator<DynamicStatObject>[] _dynamicStats;
        [SerializeField] private DynamicStatObject _lethalDynamicStat;
        [SerializeField] private float _visionRange = 10f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 720f;
        [SerializeField] private float _acceleration = 1000f;


        public DynamicStatCreator<DynamicStatObject>[] DynamicStats => _dynamicStats;
        public DynamicStatObject LethalDynamicStat => _lethalDynamicStat;
        public float VisionRange => _visionRange;
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float Acceleration => _acceleration;
    }
    [CreateAssetMenu(menuName = "FD/Units/New Unit")]
    public class UnitObject : GameDatabaseElement
    {
        [Header("Client-side")]
        [SerializeField] private InternationalString _unitName;
        [SerializeField] private InternationalString _description;
        [SerializeField] private Sprite _previewImage;

        [Header("Server-side")]
        [SerializeField] private UnitOnSceneBase _serverSidePrefab;
        [SerializeField] private ClientSide.Units.UnitOnSceneClientSide _clientSidePrefab;

        [SerializeField] private int _coinsCost;
        [SerializeField] private int _tier;

        [SerializeField] private BattleStats _battleStats;
        [SerializeField] private CommonStats _commonStats;

        [SerializeField] private UnitState _defaultState;
        [SerializeField] private Abilities.UnitAbilityObject[] _abilities;


        public InternationalString UnitName => _unitName;
        public InternationalString Description => _description;
        public Sprite PreviewImage => _previewImage;
        public UnitOnSceneBase ServerSidePrefab => _serverSidePrefab;
        public ClientSide.Units.UnitOnSceneClientSide ClientSidePrefab => _clientSidePrefab;

        public int CoinsCost => _coinsCost;
        public int Tier => _tier;

        public BattleStats BattleStats => _battleStats;
        public CommonStats CommonStats => _commonStats;

        public UnitState DefaultState => _defaultState;
        public Abilities.UnitAbilityObject[] Abilities => _abilities;
        
    }

}
