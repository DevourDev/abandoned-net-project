using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{
    public class YouLostUiHandler : UiHandlerBase
    {
        [SerializeField] private TextMeshProUGUI _mmrLostText;
        [SerializeField] private TextMeshProUGUI _expReceivedText;
        [SerializeField] private Button _goToMainMenuButton;
        [SerializeField] private Button _watchGameButton;



        private void OnDestroy()
        {
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
            _watchGameButton.onClick.RemoveListener(WatchGame);

        }
        private void Start()
        {
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
            _watchGameButton.onClick.AddListener(WatchGame);
        }

        public void Set(int mmrLost, int expReceived)
        {
            _mmrLostText.text = mmrLost.ToString();
            _expReceivedText.text = expReceived.ToString();
        }


        private void WatchGame()
        {
            gameObject.SetActive(false);
        }

        private void GoToMainMenu()
        {
            if (CM != null && CM.RealmConnection != null)
                CM.RealmConnection.Dispose();
            DevourDev.Mono.SceneManagement.DevourSceneHandler.ChangeScene("ClientSide Menu Scene"); //todo: think something like scenes database
        }
    }

}
