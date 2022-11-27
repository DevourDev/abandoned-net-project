using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FD.Utils
{
    public class FpsHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentFpsText;
        [SerializeField] private int _refreshRate = 5;


        private float _refreshTime;
        private bool _singletonedSuccessfully;

        private float _refreshTimeLeft;
        private List<float> _lastFrameTimes;

        public static FpsHandler Instance { get; private set; }

        private void Awake()
        {
            InitSingleton();
            if (!_singletonedSuccessfully)
                return;

            _lastFrameTimes = new();
        }

        private void Start()
        {
            if (!_singletonedSuccessfully)
                return;

            RecalculateRefreshTime();
        }


        private void Update()
        {
            ProcessFrame();
        }


        public void SetRefreshRate(int rate)
        {
            _refreshRate = rate;
            RecalculateRefreshTime();
        }

        private void RecalculateRefreshTime()
        {
            _refreshTime = 1f / _refreshRate;
            _refreshTimeLeft = _refreshTime;
        }

        private void ProcessFrame()
        {
            var frameTime = Time.deltaTime;
            _lastFrameTimes.Add(frameTime);
            _refreshTimeLeft -= frameTime;
            if (_refreshTimeLeft <= 0)
            {
                _refreshTimeLeft = _refreshTime;
                float avgFrameTime = _lastFrameTimes.Average();
                int avgFps = (int)(1f / avgFrameTime);
                _currentFpsText.text = avgFps.ToString();
                _lastFrameTimes.Clear();
            }
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
