using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevourDev.Base;
using DevourDev.Mono.SceneManagement;
using UnityEngine.SceneManagement;

namespace FD.Utils
{
    public class ApplicationModeSelector : MonoBehaviour
    {
        //[SerializeField] private string _exArgsPrefix = "--";
        //[SerializeField] private string _exArgsPointer = "=";

        [SerializeField] private string _defaultSceneName;
        [SerializeField] private KeyToScene[] _keysToScenes;

        private Dictionary<string, string> _scenesDic;

        private void Awake()
        {
            _scenesDic = new Dictionary<string, string>(_keysToScenes.Length);
            for (int i = 0; i < _keysToScenes.Length; i++)
            {
                _scenesDic.Add(_keysToScenes[i].Arg, _keysToScenes[i].SceneName);
            }
        }

        void Start()
        {
            var exargs = FileHandler.GetDevourArgsAsDictionary();
            string sceneName = _defaultSceneName;
            foreach (var arg in exargs)
            {
                switch (arg.Key)
                {
                    case "runmode":
                        sceneName = _scenesDic[arg.Value];
                        break;
                    default:
                        break;
                }
            }

            DevourSceneHandler.ChangeScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        [System.Serializable]
        public struct KeyToScene
        {
            public string Arg;
            public int SceneID;
            public string SceneName;
        }

    }
}
