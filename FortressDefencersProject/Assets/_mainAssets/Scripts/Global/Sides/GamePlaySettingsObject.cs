using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Game Balance Settings/Global Game Settings Object")]
    public class GamePlaySettingsObject : ScriptableObject
    {
        [SerializeField] private ShowcaseSettingsObject _showcaseSettings;
        [SerializeField] private GameEconomicsSettingsObject _economicsSettings;


        public ShowcaseSettingsObject ShowcaseSettings => _showcaseSettings;
        public GameEconomicsSettingsObject EconomicsSettings => _economicsSettings;
    }
}