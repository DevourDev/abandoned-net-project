using System;
using System.Collections.Generic;
using UnityEngine;


namespace DevourDev.MonoBase
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        public static UnityMainThreadDispatcher Instance { get; private set; }
        private bool _singletonSucceed;

        private readonly object _lockObject = new();

        private Queue<Action> _queuedActions;
        private Dictionary<object, Action> _rewritableActions;
        private List<Action> _tmpActions;

        private void Awake()
        {
            InitSingleton();

            if (!_singletonSucceed)
                return;

            _queuedActions = new Queue<Action>();
            _rewritableActions = new Dictionary<object, Action>();
            _tmpActions = new();
        }

        private void Start()
        {
            if (!_singletonSucceed)
                return;


        }

        private void Update()
        {
            ExecuteQueuedActions();
            ExecuteRewritableActions();

        }

        private void ExecuteRewritableActions()
        {
            lock (_lockObject)
            {
                if (_rewritableActions.Count == 0)
                    return;

                _tmpActions.Clear();

                foreach (var ra in _rewritableActions)
                {
                    _tmpActions.Add(ra.Value);
                }

                _rewritableActions.Clear();

                foreach (var act in _tmpActions)
                {
                    act?.Invoke();
                }
            }
        }

        private void ExecuteQueuedActions()
        {
            lock (_lockObject)
            {
                if (_queuedActions.Count == 0)
                    return;

                _tmpActions.Clear();

                while (_queuedActions.TryDequeue(out var a))
                {
                    _tmpActions.Add(a);
                }


                foreach (var act in _tmpActions)
                {
                    act?.Invoke();
                }
            }


        }


        public static void InvokeOnMainThread(Action act)
        {
            lock (Instance._lockObject)
            {
                Instance._queuedActions.Enqueue(act);
            }
        }
        public static void InvokeOnMainThread(Action act, object rewritableActionKey)
        {
            lock (Instance._lockObject)
            {
                if (!Instance._rewritableActions.TryAdd(rewritableActionKey, act))
                {
                    Instance._rewritableActions[rewritableActionKey] = act;
                }
            }
        }

        private void InitSingleton()
        {
            if (Instance != null)
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                    _singletonSucceed = false;
                }

                _singletonSucceed = true;
            }

            Instance = this;
            _singletonSucceed = true;
            //DontDestroyOnLoad(gameObject);
        }
    }
}
