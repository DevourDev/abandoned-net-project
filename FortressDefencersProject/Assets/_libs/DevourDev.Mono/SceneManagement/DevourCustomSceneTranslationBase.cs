using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DevourDev.Mono.SceneManagement
{
    public abstract class DevourCustomSceneTranslationBase : MonoBehaviour
    {
        [SerializeField] private UnityEvent _actionBeforeLoading;
        [SerializeField] private UnityEvent<float> _actionWhileLoading;
        [SerializeField] private UnityEvent _actionAfterLoading;

        private bool _notActual;


        public bool NotActual { get => _notActual; protected set => _notActual = value; }

        protected UnityEvent ActionBeforeLoading => _actionBeforeLoading;
        protected UnityEvent<float> ActionWhileLoading => _actionWhileLoading;
        protected UnityEvent ActionAfterLoading => _actionAfterLoading;


        private void Start()
        {
            _notActual = false;
            DontDestroyOnLoad(gameObject);
            StartInit();
        }


        public void TranslateToScene(int buildIndex, Action callback = null)
        {
            NotActual = true;
            StartCoroutine(CustomTranslator(buildIndex, callback));
        }


        protected virtual void StartInit() { }
        protected abstract IEnumerator CustomTranslator(int buildIndex, Action callback = null);


    }
}