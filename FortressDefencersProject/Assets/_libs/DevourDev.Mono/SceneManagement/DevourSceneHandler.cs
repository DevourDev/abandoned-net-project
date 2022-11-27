using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevourDev.Mono.SceneManagement
{
    public class DevourSceneHandler : MonoBehaviour
    {
        private static DevourSceneHandler _instance;
        private bool _singletoned;

        private bool _isBusy;


        public static DevourSceneHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    var i = new GameObject(nameof(DevourSceneHandler), typeof(DevourSceneHandler));
                    _instance = i.GetComponent<DevourSceneHandler>();
                }

                return _instance;
            }
        }


        private void Awake()
        {
            InitSingleton(true, true);
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

        private bool TryGetCustomTranslation(out DevourCustomSceneTranslation translation)
        {
            var translations = FindObjectsOfType<DevourCustomSceneTranslation>(false);

            if (translations.Length > 0)
            {
                foreach (var tr in translations)
                {
                    if (!tr.NotActual)
                    {
                        translation = tr;
                        return true;
                    }
                }
            }

            translation = null;
            return false;
        }
        public static void ChangeScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, mode);
            Instance._isBusy = false;
        }
        public static void ChangeScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneBuildIndex, mode);
            Instance._isBusy = false;
        }

        public static void ChangeSceneWithTranslator(int sceneBuildIndex)
        {
            if (Instance._isBusy)
            {
                return;
            }

            Instance._isBusy = true;

            if (Instance.TryGetCustomTranslation(out var translation))
            {
                translation.TranslateToScene(sceneBuildIndex, () => Instance._isBusy = false);
            }
            else
            {
                ChangeScene(sceneBuildIndex);
                Instance._isBusy = false;
            }
        }

    }
}