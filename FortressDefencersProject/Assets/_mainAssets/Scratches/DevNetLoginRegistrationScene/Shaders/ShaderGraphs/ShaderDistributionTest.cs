using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD
{
    public class ShaderDistributionTest : MonoBehaviour
    {
        [SerializeField] private float _maxOffset;
        [SerializeField] private int _samplesCount;
        [SerializeField] private int _directionsCount;

        [SerializeField] private GameObject _prefab;

        private GameObject[] _gos;
        private float _tmpMaxOffset;
        private float _tmpSamplesCount;
        private float _tmpDirectionsCount;

        [SerializeField] private float _deltaAngle;
        [SerializeField] private int _directionsC;

        private void OnDrawGizmos()
        { 
            //return;
            if (_directionsC < 0)
                _directionsC = 0;

            if (_deltaAngle < 0)
                _deltaAngle = 0;

            if (_directionsC == 0)
                return;

            if (_deltaAngle == 0)
                return;

            for (int dir = 0; dir < _directionsC; dir++)
            {
                Gizmos.color = Color.red;
                // Gizmos.DrawRay(Vector3.zero, new Vector3())
            }
        }


        private void Update()
        {
            //return;
            if (_maxOffset < 0)
                _maxOffset = 0;

            if (_samplesCount < 0)
                _samplesCount = 0;

            if (_directionsCount < 0)
                _directionsCount = 0;

            if (_tmpMaxOffset == _maxOffset
                && _tmpSamplesCount == _samplesCount
                && _tmpDirectionsCount == _directionsCount)
                return;

            _tmpMaxOffset = _maxOffset;
            _tmpSamplesCount = _samplesCount;
            _tmpDirectionsCount = _directionsCount;

            if (_tmpMaxOffset == 0
                && _tmpSamplesCount == 0
                && _tmpDirectionsCount == 0)
                return;

            if (_gos != null)
            {
                for (int i = 0; i < _gos.Length; i++)
                {
                    Destroy(_gos[i]);
                }
            }

            _gos = new GameObject[_directionsCount * (_samplesCount - 1)];
            int j = 0;
            float deltaOffsetLength = _maxOffset / _samplesCount;
            float deltaAngle = (float)360 / (_directionsCount);

            for (int currentDirection = 0; currentDirection < _directionsCount; currentDirection++)
            {
                double currentAngle = deltaAngle * currentDirection;
                currentAngle = System.Math.PI * currentAngle / 180.0;
                // float cos = Mathf.Cos(currentAngle);
                var cos = System.Math.Cos(currentAngle);
                float deltaOffsetX = deltaOffsetLength * (float)cos;
                //float sin = Mathf.Sin(currentAngle);
                var sin = System.Math.Sin(currentAngle);
                float deltaOffsetY = deltaOffsetLength * (float)sin;
                Vector2 deltaOffset = new(deltaOffsetX, deltaOffsetY);

                for (int currentSample = 1; currentSample < _samplesCount; currentSample++)
                {
                    var curOffset = deltaOffset * currentSample;
                    _gos[j] = Instantiate(_prefab, new Vector3(curOffset.x, curOffset.y, 0), Quaternion.identity);
                    j++;

                }
            }

            //void ColorBlur_float(float2 screenPosition, float offsetMax, float samplesCount, float directionsCount, out float3 blurredRgb)
            //{
            //	int samples = (int)samplesCount;
            //	int directions = (int)directionsCount;
            //	float deltaOffsetLength = offsetMax / samples;
            //	float deltaAngle = (float)360 / directions;

            //	float3 offsettedColor;

            //	for (int currentDirection = 0; currentDirection < directions; ++currentDirection)
            //	{
            //		float currentAngle = deltaAngle * currentDirection;
            //		float2 deltaOffset = float2(deltaOffsetLength * cos(currentAngle), deltaOffsetLength * sin(currentAngle));

            //		for (int currentSample = 0; currentSample < samples; ++currentSample)
            //		{
            //			float2 offsettedUv = screenPosition + deltaOffset * currentSample;
            //			offsettedColor += SHADERGRAPH_SAMPLE_SCENE_COLOR(offsettedUv);
            //		}

            //	}

            //	blurredRgb = offsettedColor / (samples * directions);

            //}
        }
    }
}
