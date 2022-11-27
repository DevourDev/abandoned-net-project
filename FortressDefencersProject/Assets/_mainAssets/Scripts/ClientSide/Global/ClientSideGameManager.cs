using DevourDev.Networking;
using FD.ClientSide.Units;
using FD.Networking.Client;
using FD.Networking.Realm.GamePackets;
using FD.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FD.ClientSide.Global
{

    public class ClientSideGameManager : MonoBehaviour
    {
        [SerializeField] private GameVisuals _gameVisuals;
        [SerializeField] private FD.Global.Rules.GameRulesObject _gameRules;
        [SerializeField] private ClientPersonalState _personalState;

        private ClientManager _cm;
        //private NetworkMode _netMode;
        //todo: add ShowCase


        #region oldTODO
        //TODO (after sleep): //UnitOnSceneCliendSide created.
        /*
         * Подумать, стоит ли разделять UnitOnScene(Base) на ServerSide и ClientSide классы
         * Добавить в ClientSideGameManager все поля, чтобы можно было отслеживать (добавлять и иметь доступ)
         * юнитов на Сцене, о появлении на которой сообщил Сервер; изменять (интерполировать) их Transform'ы.
         * Можно использовать UnitOnScene(Base) (не разделять на Client/Server Side), переносить из Сообщений
         * Сервера данные для UoS(B) и наблюдать за(лупой).
         * Основное преимущество использования ЮоС(Б) - абилки. Но и это не панацея, так как Evoluate()
         * будет переключать AbilityStage в CurrentAbilityStage на следующую стадию, либо, вообще,
         * выдаст ошибку, так как Время на Сервере и Клиенте - разное.
         * Плюс: если пытаться хоть что-то экономить, то нужно отслеживать предыдущие Стадии.
         * Если на Сервере Абилка закончилась (полностью выполнилась и ушла в Стадию None),
         * то нужно передать стадию None и Клиенту (а нужно ли).
         * Либо можно просто кидать в Клиента Стадии, которые нужно выполнить - 
         * Клиент зайдёт в Абилити Обджект через ДатаБазу, зайдет в Стейдж Сеттингс
         * и выполнит все ClientSide Actions. Но в таком случае, мы теряем возможность
         * сохранять состояния Абилок (так как не используем полноценно класс AbilitiesCollection).
         */
        #endregion

        public static ClientSideGameManager Instance { get; private set; }
        private bool _singletonedSuccessfully;

        private Dictionary<int, UnitOnSceneClientSide> _allUnits;
        //private int _nextItemOnSceneID;


        public FD.Global.Rules.GameRulesObject GameRules => _gameRules;
        public Dictionary<int, UnitOnSceneClientSide> AllUnits => _allUnits;
        public ClientPersonalState PersonalState => _personalState;
        public GameVisuals Visuals => _gameVisuals;


        protected ClientManager CM
        {
            get
            {
                if (_cm == null)
                {
                    _cm = ClientManager.Instance;
                }

                return _cm;
            }
        }

        private void OnDestroy()
        {
            try
            {
                //switch (_netMode)
                //{
                //    case NetworkMode.Client:
                //        CM.OnRealmMessageReceived -= CM_OnRealmMessageReceived;
                //        CM.OnRealmResponseReceived -= CM_OnRealmResponseReceived;
                //        break;
                //    default:
                //        break;
                //}
            }
            catch (Exception)
            {

            }
        }


        private void Awake()
        {
            InitSingleton();
            if (!_singletonedSuccessfully)
                return;

            _allUnits = new(1024 * 128); //     ?))

        }



        private void Start()
        {
            if (!_singletonedSuccessfully)
                return;
            //  DetectNetworkMode();
        }

        //private void DetectNetworkMode()
        //{
        //    _netMode = Networking.NetworkManager.Instance.Mode;

        //    switch (_netMode)
        //    {
        //        case NetworkMode.Client:
        //            RunAsClient();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void RunAsClient()
        //{
        //    CM.OnRealmMessageReceived += CM_OnRealmMessageReceived;
        //    CM.OnRealmResponseReceived += CM_OnRealmResponseReceived;
        //}

        //private void CM_OnRealmResponseReceived(Networking.IPacketContent p)
        //{
        //    switch (p)
        //    {
        //        case FullGameStateResponse fullGameStateResponse:
        //            HandleFullGameStateResponse(fullGameStateResponse);
        //            break;
        //        case BuyUnitResponse buyUnitResponse:
        //            HandleBuyUnitResponse(buyUnitResponse);
        //            break;
        //        case PlaceUnitResponse placeUnitResponse:
        //            HandlePlaceUnitResponse(placeUnitResponse);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void HandlePlaceUnitResponse(PlaceUnitResponse placeUnitResponse)
        //{
        //    throw new NotImplementedException();
        //}

        //private void HandleBuyUnitResponse(BuyUnitResponse buyUnitResponse)
        //{
        //    throw new NotImplementedException();
        //}

        //private void HandleFullGameStateResponse(FullGameStateResponse fullGameStateResponse)
        //{
        //    throw new NotImplementedException();
        //}

        //private void CM_OnRealmMessageReceived(Networking.IPacketContent p)
        //{
        //    switch (p)
        //    {
        //        case PersonalGameStateMessage pgsm:
        //            HandlePersonalGameStateMessage(pgsm);
        //            break;   
        //        case SharedGameStateMessage sgsm:
        //            HandleSharedGameStateMessage(sgsm);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void HandleSharedGameStateMessage(SharedGameStateMessage sgsm)
        //{
        //    throw new NotImplementedException();
        //}

        //private void HandlePersonalGameStateMessage(PersonalGameStateMessage pgsm)
        //{
        //    _personalState.CoinsWallet.ChangeBalanceToValue(pgsm.Coins);
        //    _personalState.ShowCase.UpdateShowCase(pgsm.ShowCase);
        //}

        public void ClearScene()
        {
            foreach (var item in AllUnits)
            {
                Destroy(item.Value.gameObject);
            }

            AllUnits.Clear();
        }

        //public void SetNextItemOnSceneID(int id)
        //    => _nextItemOnSceneID = id;

        public UnitOnSceneClientSide SpawnUnit(int uobjID, int itemOnSceneID, int playerID, int teamID,
            ICollection<Networking.Realm.GamePackets.DynamicStatAllValuesPacket> statsPackets)
            => SpawnUnit(uobjID, itemOnSceneID, playerID, teamID, Vector3.zero, 0, statsPackets);
        public UnitOnSceneClientSide SpawnUnit(int uobjID, int itemOnSceneID, int playerID, int teamID, Vector3 pos, float yRot,
            ICollection<Networking.Realm.GamePackets.DynamicStatAllValuesPacket> statsPackets)
            => SpawnUnit(GameRules.UnitsDatabase.GetElement(uobjID), itemOnSceneID, playerID, teamID, pos, yRot, statsPackets);
        public UnitOnSceneClientSide SpawnUnit(UnitObject uobj, int itemOnSceneID, int playerID, int teamID, Vector3 pos, float yRot,
            ICollection<Networking.Realm.GamePackets.DynamicStatAllValuesPacket> statsPackets)
        {
            var u = PlaceUnit(uobj, pos, yRot);
            _allUnits.Add(itemOnSceneID, u);
            u.UniqueID = itemOnSceneID;
            //_nextItemOnSceneID++;
            u.Init(uobj, playerID, teamID, statsPackets);

            //var u = PlaceUnit(uobj, pos, rot);
            //u.UniqueID = _allUnits.Count;
            //_allUnits.Add(u);
            //var gs = new GameSideLocal();
            //gs.

            if (u.TryGetComponent<Visuals.UnitColorChanger>(out var ucc))
            {
                ucc.Set(Visuals.GameSides.GetElement(playerID).Color);
            }

            return u;
        }

        private UnitOnSceneClientSide PlaceUnit(UnitObject uobj, Vector3 pos, float yRot)
        {
            var u = Instantiate(uobj.ClientSidePrefab, pos, Quaternion.Euler(0, yRot, 0));
            return u;
        }


        private void InitSingleton(bool destroyOnFailure = true, bool dontDestroyOnLoadOnSuccess = false)
        {
            if (Instance == this)
            {
                goto Success;
            }

            if (Instance == null)
            {
                Instance = this;
                goto Success;
            }

            _singletonedSuccessfully = false;
            if (destroyOnFailure)
            {
                Destroy(gameObject);
            }
            return;


Success:
            _singletonedSuccessfully = true;
            if (dontDestroyOnLoadOnSuccess)
            {
                DontDestroyOnLoad(gameObject);
            }
            return;
        }
    }
}