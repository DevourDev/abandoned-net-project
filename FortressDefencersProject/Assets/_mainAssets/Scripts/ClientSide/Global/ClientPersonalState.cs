using DevourDev.Base;
using DevourDev.Base.SystemExtentions;
using System.Collections.Generic;
using UnityEngine;

namespace FD.ClientSide.Global
{
    public class ClientPersonalState : MonoBehaviour
    {
        private IntegerWalletBase _coinsWallet;
        private ClientSideShowCase _showCase;


        private void Awake()
        {
            _coinsWallet = new(500);
            _showCase = new();
        }


        public IntegerWalletBase CoinsWallet => _coinsWallet;
        public ClientSideShowCase ShowCase => _showCase;
    }

    public class ClientSideShowCase
    {
        public event System.Action<List<int>> OnShowCaseChanged;

        private readonly List<int> _slots;


        public ClientSideShowCase()
        {
            _slots = new();
        }


        public void UpdateShowCase(IList<int> v)
        {
            if (_slots.ListEqual(v))
                return;

            if (v.Count != _slots.Count)
            {
                _slots.Clear();
                _slots.AddRange(v);
            }
            else
            {
                for (int i = 0; i < _slots.Count; i++)
                {
                    _slots[i] = v[i];
                }
            }

            OnShowCaseChanged(_slots);
        }

    }
}