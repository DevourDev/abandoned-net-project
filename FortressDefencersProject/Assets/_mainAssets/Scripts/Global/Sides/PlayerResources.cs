using DevourDev.Base;
using FD.Units;
using System;
using System.Collections.Generic;


namespace FD.Global.Sides
{
    public class PlayerResources : System.IDisposable
    {
        private readonly IntegerWalletBase _coinsWallet;
        private readonly Showcase _showcase;
        private readonly Dictionary<int, UnitOnSceneBase> _activeUnits;

        public PlayerResources(int maxCoins, int startCoins)
        {
            _coinsWallet = new(maxCoins, 0, startCoins);
            _showcase = new();
            _activeUnits = new();
        }


        public Dictionary<int, UnitOnSceneBase> ActiveUnits => _activeUnits;

        public IntegerWalletBase CoinsWallet => _coinsWallet;
        public Showcase Showcase => _showcase;



        public void Dispose()
        {
            foreach (var v in _activeUnits)
            {
                UnityEngine.Object.Destroy(v.Value.gameObject);
            }

            _activeUnits.Clear();   
        }
    }
}