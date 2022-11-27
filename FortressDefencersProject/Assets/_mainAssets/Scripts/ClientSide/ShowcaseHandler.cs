using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.ClientSide
{
    public class ShowcaseHandler : MonoBehaviour
    {
        [SerializeField] private Button _refreshButton;
        [SerializeField] private TextMeshProUGUI _forceRefreshCountDownText;
        [SerializeField] private TextMeshProUGUI _coinsAmountText;


        private void Start()
        {
            _refreshButton.onClick.AddListener(HandleRefreshButtonClick);
        }


        public void SetCoinsAmount(int v)
        {
            _coinsAmountText.text = $"coins: {v}";
        }

        public void SetForceRefreshCountDownValue(int seconds)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);
            var totalMins = (int)t.TotalMinutes;
            var secs = t.Seconds;
            string s = $"{(totalMins > 0 ? totalMins + ":" : string.Empty)} {secs}";
            _forceRefreshCountDownText.text = $"force refresh in: {s}";
        }

        private void HandleRefreshButtonClick()
        {

        }
    }
}
