using UnityEngine;
using FD.Networking;
using FD.Networking.Realm;
using FD.Networking.Realm.GamePackets;
using System.Threading.Tasks;
using System.Linq;
using DevourDev.MonoBase;
using System.Collections.Generic;
using DevourDev.MonoExtentions;
using System;
using DevourDev.Networking;
using FD.Networking.Gates.Packets;
using FD.Networking.Client;
using FD.ClientSide.UiHandlers;

namespace FD.ClientSide.Global
{
    public class ClientSideRealmHandler : MonoBehaviour
    {
        [SerializeField] GameOverUiHandler _gameOverUiHandler;
        [SerializeField] YouLostUiHandler _youLostUiHandler;
        [SerializeField] YouWonUiHandler _youWonUiHandler;

        private ClientSideGameManager _csgm;
        private ClientManager _cm;

        private bool _desynced;
        private bool _syncing;
        private Queue<SharedGameStateMessage> _gameStatesQ;


        protected ClientSideGameManager CSGM
        {
            get
            {
                if (_csgm == null)
                {
                    _csgm = ClientSideGameManager.Instance;
                }

                return _csgm;
            }
        }

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
                if (_cm != null)
                {
                    _cm.OnRealmMessageReceived -= CM_OnRealmMessageReceived;
                    _cm.OnRealmResponseReceived -= CM_OnRealmResponseReceived;
                }
            }
            catch (Exception) { }
        }


        private void Awake()
        {
            _desynced = false;
            _gameStatesQ = new();
        }

        private void Start()
        {
            CM.OnRealmMessageReceived += CM_OnRealmMessageReceived;
            CM.OnRealmResponseReceived += CM_OnRealmResponseReceived;
        }


        private void CM_OnRealmResponseReceived(Networking.IPacketContent p)
        {
            this.InvokeOnMainThread(() => Debug.Log(p.GetType()));
            switch (p)
            {
                case FullGameStateResponse fgsRes:
                    this.InvokeOnMainThread(() => HandleFullGameStateResponse(fgsRes));
                    break;
                case BuyUnitResponse buRes:
                    this.InvokeOnMainThread(() => HandleBuyUnitResponse(buRes)); //already handling in ClientSideShowCase.cs (not conflicting tho)
                    break;
                case PlaceUnitResponse puRes:
                    this.InvokeOnMainThread(() => HandlePlaceUnitResponse(puRes));
                    break;

                case BigPacketResponse bpRes:
                    this.InvokeOnMainThread(() => HandleBigPacketResponse(bpRes));
                    break;

                case RefreshShowCaseResponse rscRes:
                    this.InvokeOnMainThread(() =>
                    {
                        HandleRefreshShowCaseResponse(rscRes);
                    });
                    break;
                default:
                    //this.InvokeOnMainThread(() =>
                    //{
                    //    Debug.LogError("Unexpected Realm Response: " + p.GetType().ToString());
                    //});
                    break;
            }
        }

        private void CM_OnRealmMessageReceived(Networking.IPacketContent p)
        {
            this.InvokeOnMainThread(() => Debug.Log(p.GetType()));

            switch (p)
            {
                case SharedGameStateMessage sgsm:
                    if (_desynced)
                    {
                        if (_syncing)
                            _gameStatesQ.Enqueue(sgsm);

                        return;
                    }
                    this.InvokeOnMainThread(() => HandleSharedGameStateMessage(sgsm));
                    break;
                case PersonalGameStateMessage pgsm:
                    this.InvokeOnMainThread(() => HandlePersonalGameStateMessage(pgsm));
                    break;
                case GameOverMessage gom:
                    this.InvokeOnMainThread(() =>
                    {
                        HandleGameOverMessage(gom);
                    });
                    break;

                case YouLostMessage ylm:
                    this.InvokeOnMainThread(() =>
                    {
                        HandleYouLostMessage(ylm);
                    });
                    break;
                case YouWinMessage ywm:
                    this.InvokeOnMainThread(() =>
                    {
                        HandleYouWinMessage(ywm);
                    });
                    break;
                default:
                    //this.InvokeOnMainThread(() => Debug.LogError("Unexpected Realm Message: " + p.GetType().ToString()));
                    break;
            }
        }

        private void HandleYouWinMessage(YouWinMessage ywm)
        {
            _youWonUiHandler.gameObject.SetActive(true);
            _youWonUiHandler.Set(ywm.MmrChange, 228);
        }

        private void HandleYouLostMessage(YouLostMessage ylm)
        {
            _youLostUiHandler.gameObject.SetActive(true);
            _youLostUiHandler.Set(ylm.MmrChange, 228);
        }

        private void HandleGameOverMessage(GameOverMessage gom)
        {
            if (_youWonUiHandler.gameObject.activeInHierarchy)
                return;

            if (_youLostUiHandler.gameObject.activeInHierarchy)
                _youLostUiHandler.gameObject.SetActive(false);


            _gameOverUiHandler.gameObject.SetActive(true);
            _gameOverUiHandler.Set(gom.WinnerTeamID, gom.WinnerPlayerID);
        }

        private void HandleRefreshShowCaseResponse(RefreshShowCaseResponse rscRes)
        {
            if (rscRes.Result)
            {
                //animations, sounds, etc...
            }
            else
            {
                //error sound, pointing on problem (not enough coins\
            }
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                SendBigDataRequest();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (MonoSimples.TryGetWorldPositionOnMouse(Camera.main, out var floorPos, ClientSideGameManager.Instance.GameRules.GroundLayer))
                {
                    SendPlaceUnitRequest(floorPos);
                }
            }
        }

        private async void SendBigDataRequest()
        {
            var req = new BigPacketRequest();
            await CM.RealmConnection.Channel.RequestingConnection.SendRequestAsync(req);
        }

        public async void SendRefreshShowCaseRequest()
        {
            var req = new RefreshShowCaseRequest();
            await CM.SendRequestToRealm(req);
        }



        private void HandleBigPacketResponse(BigPacketResponse bpRes)
        {
            Debug.Log($"Big PacketRequest handled! {bpRes.BigDataArray.Length} bytes received!");
        }

        private void HandlePlaceUnitResponse(PlaceUnitResponse puRes)
        {
            if (puRes.Result)
                Debug.Log("Unit placed!");
            else

                Debug.Log("Unit was not placed. Error: " + puRes.FailReason.ToString());
        }

        private void HandleBuyUnitResponse(BuyUnitResponse buRes)
        {
            if (buRes.Result)
                Debug.Log("Unit bought");
            else
                Debug.Log("Unit was not bought. Error: " + buRes.FailReason.ToString());
        }

        private void HandleFullGameStateResponse(FullGameStateResponse fgsRes)
        {
            Debug.Log("full game state response received.");
            _syncing = true; //отладчик не ловит BloodTrail попробовать через VS2019. Юниты на клиенте не спавнятся.
            CSGM.ClearScene();

            if (!fgsRes.Result)
                throw new Exception("на парашу съебал!");

            SpawnNewUnits(fgsRes.PlayersResources);

            HandleSharedGameStatesQ();
            _desynced = false;
            _syncing = false;
        }


        private void HandleSharedGameStatesQ()
        {
            //todo: "lighten" up-to-dating (sync only units spawns/deathes/other major events, ignore Interpolator...)
            while (_gameStatesQ.TryDequeue(out var sgsm))
            {
                HandleSharedGameStateMessage(sgsm);
            }
        }

        private void SpawnNewUnits(PlayerNewResourcesPacket[] pRes)
        {
            foreach (var side in pRes)
            {
                for (int i = 0; i < side.ItemsOnSceneIDs.Length; i++)
                {
                    int unitObjectID = side.UnitsObjectsIDs[i];
                    int itemOnSceneID = side.ItemsOnSceneIDs[i];
                    Vector3 unitPosition = side.UpdatedTransforms[i].Position;
                    float unitYRotation = side.UpdatedTransforms[i].YRotation;
                    var dStats = side.UnitsDynamicStats[i];
                    var u = CSGM.SpawnUnit(unitObjectID, itemOnSceneID, side.SideID, side.TeamID, unitPosition, unitYRotation, dStats);
                }
            }
        }

        private async void RequestSyncPacket()
        {
            var req = new FullGameStateRequest();

            await CM.SendRequestToRealm(req);
        }

        private void HandleSharedGameStateMessage(SharedGameStateMessage sgsm)
        {
            if (sgsm.NewSidesResources.Length > 0)
            {
                SpawnNewUnits(sgsm.NewSidesResources);
            }

            if (sgsm.UpdatedUnits.Length > 0)
            {
                foreach (var uu in sgsm.UpdatedUnits)
                {
                    if (!CSGM.AllUnits.TryGetValue(uu.UnitUniqueID, out var u))
                    {
                        _desynced = true;

                        Debug.Log("desynced.");
                        RequestSyncPacket();
                        return;
                    }

                    if (uu.SideChanged)
                    {
                        u.SideData.PlayerID = uu.UpdatedSideID; //create method ChangeSide
                    }

                    if (uu.TransformChanged)
                    {
                        u.Interpolator.SetNextTurnTransform(uu.UpdatedTransform.Position, uu.UpdatedTransform.YRotation);
                        //u.Interpolator.SetNextTurnPosition(uu.UpdatedTransform.Position);
                        //u.Interpolator.SetNextTurnRotation(uu.UpdatedTransform.YRotation);
                    }
                    else
                    {
                        u.Interpolator.StayInPlace();
                    }

                    if (uu.StateChanged)
                    {
                        var state = CSGM.GameRules.UnitStatesDatabase.GetElement(uu.UpdatedState.StateID);
                        u.EvaluateClientSideState(state, uu.UpdatedState.Entered);
                    }

                    if (uu.UpdatedDynamicStats.Length > 0)
                    {
                        foreach (var uDS in uu.UpdatedDynamicStats)
                        {
                            var statObject = CSGM.GameRules.DynamicStatsDatabase.GetElement(uDS.StatID);
                            var statFound = u.DynamicStatsCollection.TryGetDynamicStat(statObject, out var s);

                            if (!statFound)
                                Debug.LogError("dynamic stat not found. Stat ID: " + uDS.StatID);

                            if (uDS.BoundsChanged)
                            {
                                s.SetMin(uDS.MinValue);
                                s.SetMax(uDS.MaxValue);
                                s.SetRegen(uDS.RegenValue);
                            }

                            if (!s.SetCurrentToValueOrClosestValid(uDS.Current, true))
                            {

                                Debug.Log("bleaaaaattt");
                            }
                        }

                    }

                    if (uu.UpdatedAbilitiesStages.Length > 0)
                    {
                        foreach (var abStage in uu.UpdatedAbilitiesStages)
                        {
                            var abObj = CSGM.GameRules.AbilitiesDatabase.GetElement(abStage.AbilityID);
                            var abState = u.AbilitiesCollection.Collection[abStage.AbilityID];
                            if (abStage.TargetChanged)
                            {
                                abStage.Target.UpdateTarget(abState.Target);
                            }
                            u.EvaluateClientSideAbilityStage(abObj, abStage.Stage);
                        }

                    }



                }
            }



            if (sgsm.Deathes.Length > 0)
            {
                foreach (var d in sgsm.Deathes)
                {
                    var u = CSGM.AllUnits[d.UnitUniqueID];
                    u.DealthEvent?.Invoke();
                    Destroy(u.gameObject);
                }
            }



        }
        private void HandlePersonalGameStateMessage(PersonalGameStateMessage pgsm)
        {
            bool succeed = CSGM.PersonalState.CoinsWallet.ChangeBalanceToValue(pgsm.Coins);

            if (!succeed)
                throw new System.Exception($"Unexpected Coins Balance value received from Server: {pgsm.Coins}");

            CSGM.PersonalState.ShowCase.UpdateShowCase(pgsm.ShowCase);
        }

        public async void SendBuyUnitRequest(int showCaseSlotID)
        {
            var req = new BuyUnitRequest();
            req.ShowCaseSlotID = showCaseSlotID;
            await CM.SendRequestToRealm(req);
        }
        public async void SendPlaceUnitRequest(Vector3 pos)
        {
            var req = new PlaceUnitRequest();
            req.SpawnPosition = pos;
            await CM.SendRequestToRealm(req);

        }

        public async void RequestTestAsync(string echoString)
        {
            var req = new TestRequest();
            req.EchoString = echoString;
            var requestedVectors = new Vector3[] { Vector3.zero, Vector3.one, Vector3.left, Vector3.forward };
            req.Vector3s = requestedVectors;
            await CM.SendRequestToRealm(req);
        }
        public async void RequestUnitPlacingAsync(Vector3 spawnPos)
        {
            var req = new PlaceUnitRequest();
            req.SpawnPosition = spawnPos;
            await CM.SendRequestToRealm(req);
        }




    }
}