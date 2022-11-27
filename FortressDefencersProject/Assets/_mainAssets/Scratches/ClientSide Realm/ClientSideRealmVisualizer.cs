using DevourDev.Base;
using DevourDev.MonoExtentions;
using FD.ClientSide.Global;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FD.ClientSide.UiHandlers
{
    public class ClientSideRealmVisualizer : MonoBehaviour
    {
        [SerializeField] private ClientPersonalState _personalState;
        [SerializeField] private TextMeshProUGUI _coinsValueText;
        [SerializeField] private ClientSideShowCase _showCase;


        private void OnDestroy()
        {
            try
            {
                _personalState.CoinsWallet.OnBalanceChanged -= CoinsWallet_OnBalanceChanged;
            }
            catch (Exception)
            {

            }

            try
            {
                _personalState.ShowCase.OnShowCaseChanged -= ShowCase_OnShowCaseChanged;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void Start()
        {
            _personalState.CoinsWallet.OnBalanceChanged += CoinsWallet_OnBalanceChanged;
            _personalState.ShowCase.OnShowCaseChanged += ShowCase_OnShowCaseChanged;
        }

        private void ShowCase_OnShowCaseChanged(List<int> v)
        {
            this.InvokeOnMainThread(() => _showCase.SetSlots(v));
        }

        private void CoinsWallet_OnBalanceChanged(object sender, int delta)
        {
            string v = ((IntegerWalletBase)sender).Balance.ToString();
            this.InvokeOnMainThread(() => _coinsValueText.text = v);
        }

    }
}
