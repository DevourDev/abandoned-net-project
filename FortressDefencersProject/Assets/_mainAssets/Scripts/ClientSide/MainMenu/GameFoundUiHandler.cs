using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{
    public class GameFoundUiHandler : UiHandlerBase
    {
        [SerializeField] private UnityEvent _onGameFoundEvent;
        [SerializeField] private TextMeshProUGUI _foundGameModeText;
        [SerializeField] private Button _acceptGameButton;


        public Button AcceptGameButton => _acceptGameButton;



        public void Set(string gameModeName)
        {
            _foundGameModeText.text = gameModeName;
            AcceptGameButton.enabled = true;
            AcceptGameButton.interactable = true;
            _onGameFoundEvent?.Invoke();
        }
    }
}
