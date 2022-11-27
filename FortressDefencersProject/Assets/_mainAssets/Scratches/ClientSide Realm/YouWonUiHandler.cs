using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{
    public class YouWonUiHandler : UiHandlerBase
    {
        [SerializeField] private TextMeshProUGUI _mmrWonText;
        [SerializeField] private TextMeshProUGUI _expReceivedText;
        [SerializeField] private Button _goToMainMenuButton;


        private void OnDestroy()
        {
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);

        }
        private void Start()
        {
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        public void Set(int mmrWon, int expReceived)
        {
            _mmrWonText.text = mmrWon.ToString();
            _expReceivedText.text = expReceived.ToString();
        }

        private void GoToMainMenu()
        {
            if (CM != null && CM.RealmConnection != null)
                CM.RealmConnection.Dispose();
            DevourDev.Mono.SceneManagement.DevourSceneHandler.ChangeScene("ClientSide Menu Scene"); //todo: think something like scenes database
        }
    }

}
