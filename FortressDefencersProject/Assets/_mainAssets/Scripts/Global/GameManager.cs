using DevourDev.MonoExtentions;
using FD.Global.Sides;
using FD.Networking;
using FD.Networking.Realm;
using FD.Networking.Realm.GamePackets;
using FD.Units;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace FD.Global
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// error code, 0 is OK
        public event Action<int> OnStartGame;
        /// </summary>
        /// <summary>
        /// error code, 0 is OK
        /// </summary>
        public event Action<int> OnEndGame;
        #region Debug
        [SerializeField] private TMPro.TextMeshProUGUI _turnTimeText;
        [SerializeField] private Transform[] _fortressesPositions;
        [SerializeField] private UnitObject _fortressObject;
        private System.Diagnostics.Stopwatch _sw;
        // [SerializeField] private List<UnitOnSceneBase> _activeUnits;
        #endregion

        //todo: create game-state multicasting (for matches spectating)
        [SerializeField] private GamePlaySettingsObject _gamePlaySettings;
        [SerializeField] private GameSidesVisualsDatabaseObject _gameSidesVisuals;
        [SerializeField] private Rules.GameRulesObject _gameRules;
        [SerializeField] private Ticker _ticker;

        private GameStateRegistrator _gameStateRegistrator;

        private bool _singletonedSuccessfully;
        private List<UnitOnSceneBase> _history;
        private Dictionary<int, UnitOnSceneBase> _hearse;
        private Dictionary<int, Sides.GameSideDefault> _sides;
        private Dictionary<int, Dictionary<int, UnitOnSceneBase>> _allUnits;

        private FD.Networking.Realm.GamePackets.FD_RealmGamePacketsResolver _realmPacketsResolver;


        public static GameManager Instance { get; private set; }

        public Rules.GameRulesObject GameRules => _gameRules;
        public Ticker Ticker => _ticker;
        public Dictionary<int, GameSideDefault> Sides => _sides;
        /// <summary>
        /// <PlayerID, Dictionary<UnitOnSceneID, UnitOnScene>
        /// </summary>
        public Dictionary<int, Dictionary<int, UnitOnSceneBase>> AllActiveUnits
        {
            get
            {
                if (_allUnits == null)
                {
                    CalculateAllUnits();
                }

                return _allUnits;
            }
        }

        public UnitOnSceneBase GetUnitByID(int id) => _history[id];
        public bool TryGetUnitByID(int id, out UnitOnSceneBase u)
        {
            if (_history.Count >= id)
            {
                u = null;
                return false;
            }

            u = _history[id];
            return true;
        }

        private void CalculateAllUnits()
        {
            if (_allUnits == null)
                _allUnits = new(_sides.Count);
            else
                _allUnits.Clear();

            foreach (var side in _sides)
            {
                _allUnits.Add(side.Value.UniqueID, side.Value.Resources.ActiveUnits);
            }
        }

        private void Awake()
        {
            InitSingleton();
            if (!_singletonedSuccessfully)
                return;

            _realmPacketsResolver = new Networking.Realm.GamePackets.FD_RealmGamePacketsResolver();
            _history = new(1024 * 8);
            _hearse = new(128);
            _sides = new();
            _gameStateRegistrator = new();

        }


        private void Start()
        {
            _sw = new();
            _ticker.OnTick += Ticker_OnTick;
            _ticker.CurrentTickrate = 10;

        }


        private void StartGame()
        {
            OnStartGame?.Invoke(0);
            //Time.fixedDeltaTime = (float)1 / 10;

            CalculateMmrChanges();
            foreach (var s in Sides)
            {
                s.Value.GenerateShowCase();
            }
            _ticker.StartTicking();
        }

        public async void EndGame(int errorCode = 0)
        {
            var goReq = new GameOverRequest();
            var waiting = RealmServerManager.Instance.GardenChannel.RequestAsync(goReq);

            //foreach (var item in _sides)
            //{

            //    try
            //    {
            //        item.Value.Dispose();
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}

            OnEndGame?.Invoke(errorCode);

            var gameOverResponseRaw = await waiting;

            if (gameOverResponseRaw is not GameOverResponse gameOverResponse)
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"Error in {nameof(GameManager)} {nameof(EndGame)}: unexpected response from Garden: {gameOverResponseRaw.GetType()} " +
                        $"({nameof(GameOverResponse)} expected.)");
                });

                return;
            }

            if (!gameOverResponse.Result)
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"Error in {nameof(GameManager)} {nameof(EndGame)}: value of {nameof(gameOverResponse.Result)} " +
                        $"is {gameOverResponse.Result}");
                });

                return;
            }

            if (gameOverResponse.CloseRealm)
            {
                Application.Quit(errorCode);
                return;
            }
            else
            {
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError($"Error in {nameof(GameManager)} {nameof(EndGame)}: value of {nameof(gameOverResponse.CloseRealm)} " +
                        $"is {gameOverResponse.CloseRealm}");
                });

                return;
            }

            switch (NetworkManager.Instance.Mode)
            {
                case DevourDev.Networking.NetworkMode.Client:
                    break;
                case DevourDev.Networking.NetworkMode.Server:
                    CloseRealmRequest crReq = new CloseRealmRequest();
                    var rawRes = RealmServerManager.Instance.GardenChannel.Request(crReq);
                    if (rawRes is CloseRealmResponse crRes
                    && crRes.Result)
                    {
                        Debug.Log("rawRes is CloseRealmResponse crRes && crRes.Result");
                        foreach (var s in _sides)
                        {
                            if (s.Value is GameSideOnline gso)
                            {
                                gso.Dispose();

                                Debug.Log("gso disposed.");
                            }
                        }
                        //Task.Delay(10_000).Wait();
                        Application.Quit(errorCode);
                    }
                    else
                    {

                        Debug.Log($"type: {rawRes.GetType()}");
                        if (rawRes is CloseRealmResponse ccrrRes)
                        {
                            Debug.Log($"Result: {ccrrRes.Result}");
                        }

                        //clean everything
                        //prepare to host new game
                    }
                    break;
                case DevourDev.Networking.NetworkMode.Host:
                    break;
                default:
                    break;
            }

        }


        //private void FixedUpdate()
        //{
        //    Ticker_OnTick(null, 10);
        //}
        public void DisconnectOnlineSide(GameSideOnline gso)
        {
            //mb not needed
        }

        private UnitOnSceneBase GenerateFortress()
        {
            if (Sides.Count > _fortressesPositions.Length)
                throw new IndexOutOfRangeException("Players amount is larger than fortress predefined spots.");

            var tr = _fortressesPositions[Sides.Count - 1];
            return PlaceUnit(_fortressObject, tr.position, tr.rotation);
        }

        public T AddGameSide<T>() where T : GameSideDefault, new()
        {

            Debug.Log($"Trying to add new Side. Current Sides count: {Sides.Count}");
            var gs = new T();
            Sides.Add(gs.UniqueID, gs);
            gs.SetTeam(Sides.Count - 1);
            var f = GenerateFortress();
            RegistrateItemOnScene(f);
            gs.RegistrateFortress(f);
            f.Init(_fortressObject, gs);
            CalculateAllUnits();

            if (Sides.Count == _fortressesPositions.Length)
            {
                Task.Run(() =>
                {
                    Task.Delay(5000);
                    StartGame();
                });
            }

            return gs;
        }
        public UnitOnSceneBase SpawnFortress(UnitObject uobj, GameSideDefault side, Vector3 pos, Quaternion rot)
        {
            var pf = PlaceUnit(uobj, pos, rot);
            RegistrateItemOnScene(pf);
            side.RegistrateFortress(pf);

            //pf.Alive = true;
            pf.Init(uobj, side);
            pf.OnDeath += (deadUnit) => _hearse.TryAdd(deadUnit.UniqueID, deadUnit);

            _gameStateRegistrator.AddNewUnit(pf);
            return pf;
        }
        public SpawnUnitResult SpawnUnit_for_pseudo(UnitObject uobj, GameSideDefault side, Vector3 pos)
        {
            if (side.Lost)
                return new SpawnUnitResult(SpawnUnitResult.FailureReasonEnum.PlayerLost);

            if (!CanBuyUnit(uobj, side))
                return new SpawnUnitResult(SpawnUnitResult.FailureReasonEnum.NotEnoughCoins);

            var rot = side.Fortress.transform.rotation;

            if (!CanPlaceUnit(uobj, pos, rot))
                return new SpawnUnitResult(SpawnUnitResult.FailureReasonEnum.BadSpawnPosition);

            BuyUnit(uobj, side);
            var u = PlaceUnit(uobj, pos, rot);
            RegistrateItemOnScene(u);
            side.AddUnit(u);

            //u.Alive = true;
            u.Init(uobj, side);
            u.OnDeath += (deadUnit) => _hearse.TryAdd(deadUnit.UniqueID, deadUnit);

            if (u.TryGetComponent<Visuals.UnitColorChanger>(out var ucc))
            {
                ucc.Set(_gameSidesVisuals.GetElement(side.UniqueID).Color);
            }

            _gameStateRegistrator.AddNewUnit(u);
            return new SpawnUnitResult(u);
        }

        private void SpawnUnit(UnitObject uobj, GameSideDefault side, Vector3 pos)
        {
            var rot = side.Fortress.transform.rotation;
            var u = PlaceUnit(uobj, pos, rot);
            RegistrateItemOnScene(u);
            side.AddUnit(u);
            u.Init(uobj, side);
            u.OnDeath += (corpse) => _hearse.TryAdd(corpse.UniqueID, corpse);

            if (u.TryGetComponent<Visuals.UnitColorChanger>(out var ucc))
                ucc.Set(_gameSidesVisuals.GetElement(side.UniqueID).Color);

            _gameStateRegistrator.AddNewUnit(u);
        }

        private void CalculateMmrChanges() // reimplement for 2+ players //move to GARDEN
        {
            Debug.LogError("MMR changes calculation started....");
            int defaultMmrChange = 30; //todo move to gameRules
            if (Sides.Count == 0)
            {
                throw new ArgumentNullException("Sides.Count == 0. CalculateMmrChanges()");
            }

            int highestMmr = int.MinValue;
            int lowestMmr = int.MaxValue;
            foreach (var s in Sides)
            {
                if (s.Value is GameSideOnline gso)
                {

                    Debug.Log($"mmr of this player is {gso.Mmr}");
                    int mmr = gso.Mmr;
                    if (mmr > highestMmr)
                    {
                        highestMmr = mmr;
                    }

                    if (mmr < lowestMmr)
                    {
                        lowestMmr = mmr;
                    }
                }
            }

            int avg = (int)((float)(highestMmr + lowestMmr) / 2);
            Debug.Log($"highestMmr: {highestMmr}, lowestMmr: {lowestMmr}, avg: {avg}");
            avg = Math.Clamp(avg, 1, int.MaxValue);
            //p1 mmr is 2000
            //p2 mmr is 1000
            //avg is 1500
            //coeff for p1 is 2000 / 1500 = 1.3333
            //p1 will get 30 / 1.3333 = 23 mmr
            //or lose 30 * 1.3333 = 39 mmr
            //coeff for p2 is 1000 / 1500 = 0.6666
            //p2 will get 30 / 0.6666 = 50 mmr
            //or lose 30 * 0.6666 = 18 mmr

            //p1 mmr is 1524
            //p2 mmr is 3005
            //avg is (1524 + 3005) / 2 = 2264
            // p1:
            // coef is 3005 / 2264 = 1.327
            // win mmr is 30 / 1.327 = 22
            // lose mmr is 30 * 1.327 = 39

            foreach (var s in Sides)
            {
                if (s.Value is GameSideOnline gso)
                {
                    float coefficient = Mathf.Clamp(gso.Mmr / avg, 0.5f, 1.5f);
                    gso.WinMmr = (int)(defaultMmrChange / coefficient);
                    gso.LoseMmr = (int)(defaultMmrChange * coefficient);
                }
            }


            /*todo when wake up:
             * client-side lose/win/end game messages handling;
             * клиент должен получать обновленное значение рейтинга при каждом заходе в игру и при каждом его изменени (его - рейтинга);
             */


        }

        public async void Eliminate(Sides.GameSideBase side)
        {
            if (side is GameSideOnline gso)
            {
                YouLostMessage ylm = new();
                ylm.MmrChange = gso.LoseMmr;
                ylm.Reason = YouLostMessage.LostReason.FortressDestroyed; // refactor!!
                gso.NetworkPlayerObject.Messaging.Message(ylm);
                PlayerLostRequest plReq = new();
                plReq.LostPlayerID = gso.PlayerInfo.AccID;
                plReq.MmrToRemove = gso.LoseMmr;
                await RealmServerManager.Instance.GardenChannel.RequestAsync(plReq);
                //if (rawResponse is PlayerLostResponse res)
                //{

                //}
            }
            GameRules.EliminationHandler.Eliminate(side);
            side.Lost = true;
            Debug.Log($"Игрок #{side.UniqueID} выбывает!");
            CheckForGameEnd();

        }



        private async void CheckForGameEnd() // need to refactor (not critical - invoking rarely)
        {
            List<Sides.GameSideDefault> aliveSides = new();


            foreach (var s in _sides)
            {
                if (!s.Value.Lost)
                    aliveSides.Add(s.Value);
            }

            if (aliveSides.Count > 1)
                return;

            Ticker.PauseTicking();
            GameOverMessage gom = new();

            if (aliveSides.Count == 0)
            {
                Debug.LogError("zero alivers?..");
                gom.WinnerTeamID = -1;
                gom.WinnerPlayerID = -1;
            }
            else
            {
                //DeclareWinner(aliveSides[0]);
                gom.WinnerPlayerID = aliveSides[0].UniqueID;
                gom.WinnerTeamID = aliveSides[0].TeamID;

                if (aliveSides[0] is GameSideOnline gso)
                {
                    YouWinMessage ywm = new();
                    ywm.MmrChange = gso.WinMmr;
                    ywm.Reason = YouWinMessage.WinReason.EnemyFortressDestroyed;
                    gso.NetworkPlayerObject.Messaging.Message(ywm);
                    PlayerWonRequest pwReq = new();
                    pwReq.WinnerPlayerID = gso.PlayerInfo.AccID;
                    pwReq.MmrToAdd = gso.WinMmr;
                    await RealmServerManager.Instance.GardenChannel.RequestAsync(pwReq);
                }
            }



            foreach (var s in Sides)
            {
                if (s.Value is GameSideOnline gso)
                {
                    gso.NetworkPlayerObject.Messaging.Message(gom);
                }
            }

            EndGame();
        }

        //private void DeclareWinner(Sides.GameSideBase winner)
        //{

        //}

        // /// <summary>
        // /// Ничья
        // /// </summary>
        //private void DeclareDraw()
        //{
        //    Debug.LogError("Match result: DRAW!");
        //    throw new NotImplementedException("DRAW is not implemented");

        //}


        private bool CanBuyUnit(UnitObject uobj, Sides.GameSideDefault side)
        {
            return side.Resources.CoinsWallet.CanSpend(uobj.CoinsCost);
        }
        private void BuyUnit(UnitObject uobj, Sides.GameSideDefault side)
        {
            side.Resources.CoinsWallet.Spend(uobj.CoinsCost);
        }

        private bool CanPlaceUnit(UnitObject uobj, Vector3 pos, Quaternion rot)
        {
            return DevourDev.MonoBase.MonoSimples.CheckGameObjectTouchesNothing(uobj.ServerSidePrefab.gameObject, pos, rot, GameRules.UnitsLayer);
        }
        private UnitOnSceneBase PlaceUnit(UnitObject uobj, Vector3 pos, Quaternion rot)
        {
            UnitOnSceneBase u = Instantiate(uobj.ServerSidePrefab, pos, rot);
            return u;
        }

        private void RegistrateItemOnScene(UnitOnSceneBase u)
        {
            u.UniqueID = _history.Count;
            _history.Add(u);
        }

        private void Ticker_OnTick(object sender, int tps)
        {
            _sw.Restart();
            try
            {
                MakeGlobalTurn();
                HandleRequests();
                SendResponses();
                SendGameStates();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in Ticker_OnTick: " + ex);
                _gameStateRegistrator.Clear();
            }

            _sw.Stop();
            _turnTimeText.text = "turntime: " + _sw.Elapsed.TotalMilliseconds.ToString();
        }

        private void SendGameStates()
        {
            var encodedGameStateData = _gameStateRegistrator.GetSharedGameStateMessageData();
            foreach (var side in Sides)
            {
                if (side.Value is GameSideOnline gso && gso.ConnectionStatus == OnlineConnectionStatus.OK)
                {
                    var messaging = gso.NetworkPlayerObject.Messaging;
                    messaging.Connection.SendData(encodedGameStateData);
                    var pgs = new PersonalGameStateMessage();
                    pgs.Coins = gso.Resources.CoinsWallet.Balance;
                    pgs.ShowCase = side.Value.Resources.Showcase.Slots.ToArray(); //add bool 'ShowCaseChanged'...
                    messaging.Message(pgs);
                }
            }
        }

        private void SendResponses()
        {
            foreach (var side in Sides)
            {
                side.Value.SendQueuedResponses();
            }
        }

        private void HandleRequests()
        {
            foreach (var side in Sides)
            {
                while (side.Value.TryGetRequest(out var req))
                {
                    var res = HandleRequest(req, side.Value);
                    side.Value.AddResponse(res);
                }
            }
        }

        private void MakeGlobalTurn()
        {
            foreach (var side in AllActiveUnits)
            {
                foreach (var u in side.Value)
                {
                    //if (u.Value.Alive)
                    u.Value.MakeTurn();
                }
            }

            BuryCorpses();
            SyncTransforms();
            SyncDynamicStats();
        }

        public void RegistrateAbilityStage(Units.Ai.UnitAi ai, Units.Abilities.UnitAbilityState state)
        {
            _gameStateRegistrator.AddAbilityStateUpdate(ai.ServerSideUnit, state);
        }
        private void SyncDynamicStats()
        {
            foreach (var side in AllActiveUnits)
            {
                foreach (var u in side.Value)
                {
                    _gameStateRegistrator.AddDynamicStatUpdate(u.Value);
                }
            }
        }

        private void SyncTransforms()
        {
            foreach (var side in AllActiveUnits)
            {
                foreach (var u in side.Value)
                {
                    _gameStateRegistrator.AddTransformUpdate(u.Value);
                }
            }
        }

        private IPacketContent HandleRequest(IPacketContent c, GameSideDefault side)
            => c switch
            {
                BigPacketRequest => HandleBigPacketRequest(),
                TestRequest tr => HandleTestRequest(tr),
                BuyUnitRequest buReq => HandleBuyUnitRequest(buReq, side),
                PlaceUnitRequest puReq => HandlePlaceUnitRequest(puReq, side),
                FullGameStateRequest fgsReq => HandleFullGameStateRequest(fgsReq, side),
                RefreshShowCaseRequest rscReq => HandleRefreshShowCaseRequest(rscReq, side),
                _ => throw new NotImplementedException("Unexpected packet content. ID: " + c.UniqueID),
            };

        private IPacketContent HandleRefreshShowCaseRequest(RefreshShowCaseRequest rscReq, GameSideDefault side)
        {
            var response = new RefreshShowCaseResponse();
            response.Result = side.Resources.CoinsWallet.CanSpend(1);

            if (!response.Result)
            {
                response.FailReason = RefreshShowCaseResponse.FailureReason.NotEnoughCoins;
                return response;
            }

            side.Resources.CoinsWallet.Spend(1);
            side.GenerateShowCase();

            
            return response;

        }

        private IPacketContent HandleBigPacketRequest()
        {
            var response = new BigPacketResponse();
            response.BigDataArray = new byte[UnityEngine.Random.Range(100_000, 1000_000)];
            new System.Random().NextBytes(response.BigDataArray);
            return response;
        }

        /// <summary>
        /// ДОРОГО!!! (не особо)
        /// </summary>
        /// <param name="fgsReq"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        private IPacketContent HandleFullGameStateRequest(FullGameStateRequest fgsReq, GameSideDefault side)
        {
            var prps = new List<PlayerNewResourcesPacket>(Sides.Count);
            var response = new FullGameStateResponse(prps);
            response.Result = true;

            foreach (var s in AllActiveUnits)
            {
                var prp = new PlayerNewResourcesPacket(true);
                prps.Add(prp);
                foreach (var u in s.Value)
                {
                    prp.AddUnit(u.Value);
                }
            }

            PersonalGameStateMessage pgsm = new();
            pgsm.Coins = side.Resources.CoinsWallet.Balance;
            pgsm.ShowCase = side.Resources.Showcase.Slots.ToArray(); //change to encode List, decode Array

            Debug.LogError($"Full Game State Response sent to {side.UniqueID}");

            return response;
        }

        private IPacketContent HandlePlaceUnitRequest(PlaceUnitRequest puReq, GameSideDefault side)
        {
            var response = new PlaceUnitResponse();
            var pos = puReq.SpawnPosition;
            var unit = side.UnitInHand;
            if (unit == null)
            {
                response.Result = false;
                response.FailReason = PlaceUnitResponse.FailReasons.NothingToPlace;
                return response;
            }
            response.Result = CanPlaceUnit(side.UnitInHand, pos, side.Fortress.transform.rotation);
            if (!response.Result)
            {
                response.FailReason = PlaceUnitResponse.FailReasons.AreaIsBusy;
            }
            else
            {
                SpawnUnit(side.UnitInHand, side, pos);
                side.UnitInHand = null;
            }

            return response;
        }

        private IPacketContent HandleBuyUnitRequest(BuyUnitRequest buReq, GameSideDefault side)
        {
            var response = new BuyUnitResponse();

            if (side.Lost)
            {
                response.Result = false;
                response.FailReason = BuyUnitResponse.FailReasons.PlayerLost;
                return response;
            }

            if (side.UnitInHand != null)
            {
                response.Result = false;
                response.FailReason = BuyUnitResponse.FailReasons.UnitInHand;
                return response;
            }

            var unitObjectID = side.Resources.Showcase.Slots[buReq.ShowCaseSlotID];
            var unitObject = GameRules.UnitsDatabase.GetElement(unitObjectID);
            response.Result = CanBuyUnit(unitObject, side);

            if (!response.Result)
            {
                response.FailReason = BuyUnitResponse.FailReasons.NotEnoughCoins;
            }
            else
            {
                BuyUnit(unitObject, side);
                side.UnitInHand = unitObject;
                side.Resources.Showcase.RemoveAtSlot(buReq.ShowCaseSlotID);
            }
            return response;
        }

        private IPacketContent HandleTestRequest(TestRequest r)
        {
            Debug.Log($"TestRequest received. String: {r.EchoString}");
            var response = new TestResponse
            {
                EchoString = r.EchoString,
                VectorsAmount = r.Vector3s.Length,
                Vector3s = r.Vector3s,
                Result = true
            };
            return response;
        }

        private void BuryCorpses()
        {
            if (_hearse.Count == 0)
                return;


            int corpsesBefore = _hearse.Count;

            foreach (var c in _hearse)
            {
                c.Value.Owner.RemoveUnit(c.Value);
                _gameStateRegistrator.AddDeath(c.Value);
                Destroy(c.Value.gameObject);
            }

            int corpsesAfter = _hearse.Count;

            if (corpsesBefore != corpsesAfter)
            {
                Debug.LogError($"АААА НАСРАЛИ В КАТАФАЛК!!! було {corpsesBefore}, стало {corpsesAfter}");
            }


            _hearse.Clear();
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