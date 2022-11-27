using UnityEngine;
using TMPro;
using DevourDev.Base;

namespace FD.ClientSide.Global
{
    [System.Obsolete("Use ClientSide Realm Handler")]
    public class ClientUiVisualizer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsText;


        private void Start()
        {
            var csgm = ClientSideGameManager.Instance;
            csgm.PersonalState.CoinsWallet.OnBalanceChanged += CoinsWallet_OnBalanceChanged;
        }

        private void CoinsWallet_OnBalanceChanged(object sender, int delta)
        {
            var wallet = (IntegerWalletBase)sender;
            _coinsText.text = wallet.Balance.ToString();

            if (delta < 0)
            {
                Debug.Log($"{delta} coins spent.");
            }
            else
            {
                Debug.Log($"{delta} coins earned!");
            }

        }
    }
}