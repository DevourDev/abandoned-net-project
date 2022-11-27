using DevourDev.MonoExtentions;
using FD.ClientSide.Global;
using FD.Networking.Client;
using FD.Networking.Garden.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{

    public class ClientSideMainMenuHandler : UiHandlerBase
    {
        [SerializeField] private TextMeshProUGUI _playersOnlineText;
        [SerializeField] private TextMeshProUGUI _activeSearchersText;


        [SerializeField] private UnityEvent _onGameAccepted;
        [SerializeField] private UnityEvent _onConnectedToRealm;
        [SerializeField] private SearchingGameUiHandler _searchingGameCanvasHandler;
        [SerializeField] private GameFoundUiHandler _gameFoundCanvasHandler;

        [SerializeField] private Button _enterMatchMakingQueueButton;

        [SerializeField] private float _onlineStatsRequestPeriod = 15f;

        private float _onlineStatsRequestCooldown;


        private void Start()
        {
            CM.OnGardenMessageReceived += CM_OnGardenMessageReceived;
            _enterMatchMakingQueueButton.onClick.AddListener(HandleEnterMMQButtonClick);
            // handle quit queue button _searchingGameCanvasHandler.onClick.AddListener(EnterMatchMakingQueue);
            _gameFoundCanvasHandler.AcceptGameButton.onClick.AddListener(AcceptFoundGame);

        }

        private void OnDestroy()
        {
            CM.OnGardenMessageReceived -= CM_OnGardenMessageReceived;
            _enterMatchMakingQueueButton.onClick.RemoveListener(HandleEnterMMQButtonClick);
            // handle quit queue button _searchingGameCanvasHandler.onClick.RemoveListener(EnterMatchMakingQueue);
            _gameFoundCanvasHandler.AcceptGameButton.onClick.RemoveListener(AcceptFoundGame);

        }

        private void Update()
        {
            UpdateOnlinersStat();
        }

        private void UpdateOnlinersStat()
        {
            _onlineStatsRequestCooldown -= Time.deltaTime;

            if (_onlineStatsRequestCooldown < 0)
            {
                _onlineStatsRequestCooldown = _onlineStatsRequestPeriod;
                RequestOnlinersStatUpdate();
            }
        }

        private async void RequestOnlinersStatUpdate()
        {
            var req = new OnlinePlayersStatsRequest();
            var rawRes = await CM.RequestGarden(req);

            if (rawRes is not OnlinePlayersStatsResponse res)
            {
                //((((

                return;
            }

            if (!res.Result)
            {
                //((((

                return;
            }

            _playersOnlineText.text = res.OnlinersCount.ToString();
            _activeSearchersText.text = res.SearchersCount.ToString();
        }

        private void CM_OnGardenMessageReceived(Networking.IPacketContent p, Networking.ListeningConnection c)
        {

            this.InvokeOnMainThread(() =>
            {
                Debug.Log($"{nameof(ClientSideMainMenuHandler)}:: Garden message received: {p.GetType()}");
            });



            switch (p)
            {
                case FD.Networking.Garden.Packets.GameFoundMessage gfm:
                    this.InvokeOnMainThread(() => HandleGameFoundMessage(gfm));
                    break;

                case FD.Networking.Garden.Packets.RealmInviteMessage realmInvite:
                    this.InvokeOnMainThread(() => HandleRealmInviteMessage(realmInvite));
                    break;
                default:
                    break;
            }
        }

        private async void HandleRealmInviteMessage(Networking.Garden.Packets.RealmInviteMessage realmInvite)
        {
            Debug.LogError("HandleRealmInviteMessage: enter method.");
            if (!await CM.TryConnectToRealmAsync(realmInvite.RealmIPEP, realmInvite.RealmSessionKey))
            {
                Debug.LogError("TryConnectToRealmAsync -> FALSE");
                this.InvokeOnMainThread(() =>
                {
                    Debug.LogError("TryConnectToRealmAsync -> FALSE (from Dispatcher)");
                });
                return;
            }
            Debug.LogError("HandleRealmInviteMessage: connected to realm.");

            Debug.LogError("Now we are going to Gaming Scene...");
            _onConnectedToRealm?.Invoke();
        }


        private async void HandleEnterMMQButtonClick()
        {
            _enterMatchMakingQueueButton.gameObject.SetActive(false);
            var req = new FD.Networking.Garden.Packets.EnterFindGameQueueRequest
            {
                Mode = Networking.Garden.Packets.GameMode.Default,
                SessionKey = CM.GardenConnection.SessionKey
            };

            var enterQRequesting = CM.RequestGarden(req);
            var rawResponse = await enterQRequesting;
            if (rawResponse is not FD.Networking.Garden.Packets.EnterFindGameQueueResponse res)
            {
                Debug.LogError($"Unexpected response: {rawResponse.GetType().Name}. Expected: {nameof(FD.Networking.Garden.Packets.EnterFindGameQueueResponse)}.");
                return;
            }

            if (!res.Result)
            {
                _enterMatchMakingQueueButton.gameObject.SetActive(true);
                Debug.Log("Request declined. Reason: " + res.FailReason.ToString());
                return;
            }

            _searchingGameCanvasHandler.QuitMatchMakingQueueButton.onClick.AddListener(() =>
            {
                _enterMatchMakingQueueButton.gameObject.SetActive(true);
            });

            _searchingGameCanvasHandler.gameObject.SetActive(true);
            _searchingGameCanvasHandler.StartSearching();
        }

        private void HandleGameFoundMessage(FD.Networking.Garden.Packets.GameFoundMessage gfm)
        {
            _gameFoundCanvasHandler.gameObject.SetActive(true);
            _gameFoundCanvasHandler.Set(gfm.FoundGameMode.ToString());
        }

        private async void AcceptFoundGame()
        {
            _gameFoundCanvasHandler.AcceptGameButton.interactable = false;
            var req = new FD.Networking.Garden.Packets.AcceptGameRequest();
            var rawResponse = await CM.RequestGarden(req);
            if (rawResponse is not FD.Networking.Garden.Packets.AcceptGameResponse res)
            {
                Debug.LogError($"Unexpected response: {rawResponse.GetType().Name}. Expected: {nameof(FD.Networking.Garden.Packets.AcceptGameResponse)}.");
                return;
            }

            if (!res.Result)
            {
                Debug.LogError("Request declined. Reason: " + res.FailReason.ToString());
                return;
            }

            _onGameAccepted?.Invoke();


        }

        private async void StartGameVsBot()
        {
            throw new System.NotImplementedException("Game vs Bot is not implemented");
            //_gameVsBotCanvasHandler.gameObject.SetActive(true);
            //_gameVsBotCanvasHandler.DoSomething();
        }

        private async void EnterMyCollection()
        {
            throw new System.NotImplementedException("My Collection is not implemented");
            //_myCollectionCanvasHandler.gameObject.SetActive(true);
            // _myCollectionCanvasHandler.DoSomething();
        }
    }
}
