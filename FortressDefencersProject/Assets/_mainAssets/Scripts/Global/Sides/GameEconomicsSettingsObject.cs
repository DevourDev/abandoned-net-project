using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Game Balance Settings/Economics")]
    public class GameEconomicsSettingsObject : ScriptableObject
    {
        [SerializeField] private int _startCoinsAmount;
        [SerializeField] private int _maxCoinsAmount;


        public int StartCoins => _startCoinsAmount;
        public int MaxCoins => _maxCoinsAmount;
    }
}