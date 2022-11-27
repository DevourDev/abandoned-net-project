using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Utils
{
    public class PerformanceHandler : MonoBehaviour //work in progress
    {
        [SerializeField, InspectorName("Normal FPS counter")] private int _normalFps;
        [SerializeField, InspectorName("Hight FPS counter")] private int _highFps;
        [SerializeField] private int _averageCalculationFramesCount = 10;

        [SerializeField] private GameObject[] _heavyVisuals;
        [SerializeField] private UnityEvent _lowPerformanceDetectedEvent;
        [SerializeField] private UnityEvent _highPerformanceDetectedEvent;

        /*todo:
         * �������� �������� (https://docs.unity3d.com/ScriptReference/Resolution-refreshRate.html)
         * ������ ����. ������������ ����� ������������ ����������� �������� �������� ������ � �������
         * ���� ������� �� ������� �������� ������� �������� (���� ������ ���������� ������� ���������) ���� ������:
         * ����� ��������� _lowPerforformanceDetectedEvent[i++] (������� ������������� ���������)
         * ���� ����� �� "������������"
         * ���������� ������������ ������������������
         * ���� ������ - _lowPerforformanceDetectedEvent[i++]
         * ���� ��� ���������� ���� �������� �������� �� ��������� ���� ������ - ������� ��������������,
         * ��, ��-��������, ��������� _lowPerforformanceDetectedEvent ������ ���� ������� �� ����� � 1 ������ � �������,
         * ������� � ��������� �������� � ���������� � ������� ������ (������������) ������������������. ����� ��������
         * ������ ������, ���� �������������� �������� ���������� �� �������.
         */

        private float _normalFrameTime;
        private float _performantFrameTime;

        private float[] _lastFrames;
        private int _lastFramesIndex;

        private bool _heavyVisualsDisabled;
        private bool _activeLowPerformance;

        private void Awake()
        {
            _normalFrameTime = (float)1 / _normalFps;
            _performantFrameTime = (float)1 / _highFps;
            _lastFrames = new float[_averageCalculationFramesCount];
            _lastFramesIndex = 0;
            _heavyVisualsDisabled = false;
        }

        private void Update()
        {
            if (_activeLowPerformance)
                return;

            if (Time.time < 5)
                return;

            _lastFramesIndex++;
            float ft = Time.deltaTime;
            _lastFrames[_lastFramesIndex] = ft;

            if (_lastFramesIndex == _averageCalculationFramesCount - 1)
            {
                _lastFramesIndex = 0;
                var avg = _lastFrames.Average();
                if (avg > _normalFrameTime)
                {
                    _activeLowPerformance = true;
                    if (!_heavyVisualsDisabled)
                    {
                        foreach (var hv in _heavyVisuals)
                        {
                            hv.SetActive(false);
                        }
                        _heavyVisualsDisabled = true;
                    }

                    _lowPerformanceDetectedEvent?.Invoke();
                }
            }
        }

    }
}
