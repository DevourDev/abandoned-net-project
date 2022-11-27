using DevourDev.Base;
using System;
using UnityEngine;

namespace FD.Global
{
    public class Ticker : MonoBehaviour, ITicker
    {
        public event EventHandler<int> OnTick;

        private int _tickrate;
        private double _nextTickTime;
        private double _crossTicksTime;


        public int CurrentTickrate { get => _tickrate; set => SetTickrate(value); }
        public bool Ticking { get; protected set; }


        private void Update()
        {
            Tick();
        }

        private void Tick()
        {
            if (!Ticking)
                return;

            double t = Time.timeAsDouble;
            if (t >= _nextTickTime)
            {
                _nextTickTime = t + _crossTicksTime;
                OnTick?.Invoke(this, _tickrate);
            }
        }

        public void StartTicking()
        {
            Ticking = true;
        }

        public void PauseTicking()
        {
            Ticking = false;
        }


        private void SetTickrate(int value)
        {
            _tickrate = value;
            RecalculateCrossTickTime();
        }

        private void RecalculateCrossTickTime()
        {
            _crossTicksTime = (double)1 / _tickrate;
        }
    }
}