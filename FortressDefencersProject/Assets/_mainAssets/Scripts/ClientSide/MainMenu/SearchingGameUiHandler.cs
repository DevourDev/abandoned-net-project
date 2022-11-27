using FD.Networking.Garden.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{

    public class SearchingGameUiHandler : UiHandlerBase
    {
        [SerializeField] private TextMeshProUGUI _searchingDurationText;
        [SerializeField] private TextMeshProUGUI _searchingGameModsText;
        [SerializeField] private Button _quitMatchMakingQueueButton;


        private bool _countingDuration;
        private double _searchingStartedTime;
        private int _lastSearchingSecond;
        private StringBuilder _timeInQueueSB;


        public Button QuitMatchMakingQueueButton => _quitMatchMakingQueueButton;


        private void OnDisable()
        {
            _quitMatchMakingQueueButton.onClick.RemoveAllListeners();
        }
        private void OnEnable()
        {
            if (_timeInQueueSB == null)
                _timeInQueueSB = new();
            else
                _timeInQueueSB.Clear();

            _quitMatchMakingQueueButton.onClick.AddListener(HandleQuitMMQ);

        }



        private async void HandleQuitMMQ()
        {
            QuitFindGameQueueRequest req = new();
            var rawRes = await CM.RequestGarden(req);
            if (rawRes is not QuitFindGameQueueResponse res)
            {
                Debug.LogError($"unexpected response {rawRes.GetType()}. {nameof(QuitFindGameQueueResponse)} expected.");
                return;
            }

            if (!res.Result)
            {

                Debug.Log("asdasfsa");
            }

            gameObject.SetActive(false);
        }

        public void StartSearching()
        {
            _searchingStartedTime = Time.realtimeSinceStartupAsDouble;
            _lastSearchingSecond = (int)_searchingStartedTime;
            _countingDuration = true;
            _searchingGameModsText.text = $"Стандартный режим";
        }

        private void Update()
        {
            UpdateSearchingDuration();
        }

        private void UpdateSearchingDuration()
        {
            if (!_countingDuration)
                return;

            var now = Time.realtimeSinceStartupAsDouble;
            var second = (int)now;

            if (_lastSearchingSecond == second)
                return;


            _timeInQueueSB.Clear();
            //pizdek...
            _timeInQueueSB.Append("время в очереди: ");
            _lastSearchingSecond = second;

            int minutes = (int)((now - _searchingStartedTime) / 60);
            int seconds = (int)((now - _searchingStartedTime) % 60);

            if (minutes < 10)
            {
                _timeInQueueSB.Append('0');
            }
            _timeInQueueSB.Append(minutes);
            _timeInQueueSB.Append(':');
            if (seconds < 10)
            {
                _timeInQueueSB.Append('0');
            }
            _timeInQueueSB.Append(seconds);
            _searchingDurationText.text = _timeInQueueSB.ToString();
        }
    }
}
