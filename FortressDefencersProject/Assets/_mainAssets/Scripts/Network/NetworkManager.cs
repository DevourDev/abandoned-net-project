using DevourDev.Networking;
using UnityEngine;

namespace FD.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private NetworkMode _debug_initialMode = NetworkMode.Host;


        public static NetworkManager Instance { get; private set; }
        private bool _singletonedSuccessfully;


        public NetworkMode Mode { get; protected set; }


        private void Awake()
        {
            InitSingleton();
            if (!_singletonedSuccessfully)
                return;

            Mode = _debug_initialMode;
        }


        public void InitAsClient()
        {
            Mode = NetworkMode.Client;
        }
        public void InitAsServer()
        {
            Mode = NetworkMode.Server;
        }
        public void InitAsHost()
        {
            Mode = NetworkMode.Host;
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
}
