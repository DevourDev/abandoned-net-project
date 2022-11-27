using DevourDev.MonoExtentions;
using FD.Networking.Client;
using FD.Networking.Garden.Packets;
using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FD.ClientSide.UiHandlers
{
    public class WaitingAllPlayersReadyCanvasHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _readyPlayersStatusText;

        private bool[] _lastReadiers;


        private void OnEnable()
        {
            _lastReadiers = null;
            ClientManager.Instance.OnGardenMessageReceived += CM_OnGardenMessageReceived;
        }

        private void OnDisable()
        {
            ClientManager.Instance.OnGardenMessageReceived -= CM_OnGardenMessageReceived;
        }
            

        private void CM_OnGardenMessageReceived(Networking.IPacketContent p, Networking.ListeningConnection arg2)
        {
            this.InvokeOnMainThread(() =>
            {
                Debug.Log($"{nameof(WaitingAllPlayersReadyCanvasHandler)}:: Garden message received: {p.GetType()}");
            });

            switch (p)
            {
                case FoundGameStateMessage fgsm:
                    HandleFoundGameStateMessage(fgsm);
                    break;
                default:
                    break;
            }
        }

        private void HandleFoundGameStateMessage(FoundGameStateMessage fgsm)
        {
            UpdateReadyPlayers(fgsm.PlayersAcceptions);
        }


        public void UpdateReadyPlayers(bool[] readies)
        {
            if (_lastReadiers != null && _lastReadiers.SequenceEqual(readies))
                return;

            _lastReadiers = readies;

            StringBuilder sb = new();
            for (int i = 0; i < readies.Length; i++)
            {
                sb.Append($"Игрок {i + 1} {(readies[i] ? "готов" : "не готов")}{(i == readies.Length - 1 ? "\n" : ".")}");
            }
            _readyPlayersStatusText.text = sb.ToString();
        }
    }
}
