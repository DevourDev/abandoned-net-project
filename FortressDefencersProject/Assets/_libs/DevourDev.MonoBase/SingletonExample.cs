using UnityEngine;

namespace DevourDev.MonoBase
{
    public class SingletonExample : MonoBehaviour
    {
        public static SingletonExample Instance { get; private set; }
        private bool _singletonedSuccessfully;


        private void Awake()
        {
            InitSingleton();
            if (!_singletonedSuccessfully)
                return;
        }

        private void InitSingleton(bool destroyOnFailure = true, bool dontDestroyOnLoadOnSuccess = false)
        {
            if (Instance == this)
            {
                goto Success;
            }

            if (Instance == null)
            {
                Instance = this;
                goto Success;
            }

            _singletonedSuccessfully = false;
            if (destroyOnFailure)
            {
                Destroy(gameObject);
            }
            return;


        Success:
            _singletonedSuccessfully = true;
            if (dontDestroyOnLoadOnSuccess)
            {
                DontDestroyOnLoad(gameObject);
            }
            return;
        }
    }

    public class AutoSingletonExample : MonoBehaviour
    {
        private static AutoSingletonExample _instance;
        private bool _singletoned;


        public static AutoSingletonExample Instance
        {
            get
            {
                if (_instance == null)
                {
                    var i = new GameObject(nameof(AutoSingletonExample), typeof(AutoSingletonExample));
                    _instance = i.GetComponent<AutoSingletonExample>();
                }

                return _instance;
            }
        }


        private void Awake()
        {
            InitSingleton();
            if (!_singletoned)
                return;
        }

        private void InitSingleton(bool destroyOnFailure = true, bool dontDestroyOnLoadOnSuccess = false)
        {
            if (_instance == this)
            {
                goto Success;
            }

            if (_instance == null)
            {
                _instance = this;
                goto Success;
            }

            _singletoned = false;
            if (destroyOnFailure)
            {
                Destroy(gameObject);
            }
            return;


        Success:
            _singletoned = true;
            if (dontDestroyOnLoadOnSuccess)
            {
                DontDestroyOnLoad(gameObject);
            }
            return;
        }
    }
}