using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Game Balance Settings/Showcase")]
    public class ShowcaseSettingsObject : ScriptableObject
    {
        [SerializeField] private int _slotsAmount = 5;
        [SerializeField] private int[] _tiers;
        [SerializeField] private bool _playerCanRefreshShowcase = true;
        [SerializeField] private int _refreshCost = 2;
        [SerializeField] private int _userRefreshCooldown = 0;
        [SerializeField] private int _autoRefreshPeriodTime = 20;

        public int SlotsAmount => _slotsAmount;
        public int[] Tiers => _tiers;
        public bool PlayerCanRefreshShowcase => _playerCanRefreshShowcase;
        public int RefreshCost => _refreshCost;
        public int UserRefreshCooldown => _userRefreshCooldown;
        public int AutoRefreshPeriodTime => _autoRefreshPeriodTime;

        //потом подумать про СТАДИИ (подготовка, мидгейм, АД, DOOM...)
    }
}