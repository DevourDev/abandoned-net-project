using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Utils
{
    public class SystemTimeDisplayer : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _text;

        private int _lastSecond = -1;
        void Update()
        {
            var now = System.DateTime.Now;

            if (_lastSecond == now.Second)
                return;

            _lastSecond = now.Second;
            _text.text = System.DateTime.Now.ToLongTimeString();
        }
    }
}
