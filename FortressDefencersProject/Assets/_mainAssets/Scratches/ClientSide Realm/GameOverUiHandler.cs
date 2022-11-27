using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{
    public class GameOverUiHandler : UiHandlerBase
    {
        [SerializeField] private TextMeshProUGUI _winnerTeamText;
        [SerializeField] private TextMeshProUGUI _winnerPlayerText;
        //[SerializeField] private TextMeshProUGUI _personalStateText;


        [SerializeField] private Button _goToMainMenuButton;



        private void OnDestroy()
        {
            _goToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        private void Start()
        {
            _goToMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        public void Set(int winnerTeamID, int winnerPlayerID)
        {
            var winnerTeam = CGM.Visuals.Teams.GetElement(winnerTeamID);
            _winnerTeamText.text = winnerTeam.TeamName.GetAnyNameOrDefault("_ERROR_");
            _winnerTeamText.color = winnerTeam.Color;

        }

        private void GoToMainMenu()
        {
            if (CM != null && CM.RealmConnection != null)
                CM.RealmConnection.Dispose();

            DevourDev.Mono.SceneManagement.DevourSceneHandler.ChangeScene("ClientSide Menu Scene"); //todo: think something like scenes database
        }
    }

}
