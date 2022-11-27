using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD
{
    public class FastAccess : MonoBehaviour
    {
        public static FastAccess Instance { get; private set; }
        private bool _isSingleton;

        [SerializeField] private int _test;


        public static int GetTest => Instance._test;


        private void Awake()
        {
            _isSingleton = InitSingleton();
            if (!_isSingleton)
                return;
        }



        private bool InitSingleton(bool setDontDestroyOnLoadOnSucceed = false, bool destroyOnFail = true)
        {
            bool succeed = true;

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                succeed = false;
            }

            if (succeed && setDontDestroyOnLoadOnSucceed)
                DontDestroyOnLoad(gameObject);

            if (!succeed && destroyOnFail)
                Destroy(gameObject);

            return succeed;
        }
    }
}
