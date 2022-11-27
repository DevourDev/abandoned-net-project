using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD
{
    public class FollowPath : MonoBehaviour
    {
        [SerializeField] private PathPoint[] _points;
        [SerializeField] private float _speed;
        [SerializeField] private float _positionError = 0.03f;

        private int _curPointInd;

        private Vector3 _crossPointsDistanceVector;
        private float _crossPointsDistance;
        private float _distanceLeft;

        private Vector3 NextPosition => _points[_curPointInd].Transform.position;
        private Vector3 PrevPosition => _points[_curPointInd == 0 ? _points.Length - 1 : _curPointInd - 1].Transform.position;

        private void Start()
        {
            PathPoint startPoint = default;

            for (int i = 0; i < _points.Length; i++)
            {
                PathPoint p = _points[i];
                if (p.IsStartPoint)
                {
                    startPoint = p;
                    _curPointInd = i;
                    break;
                }
            }

            transform.position = startPoint.Transform.position;

            CalculateNextPointIndex();
            CalculateCrossPoints();
        }

        private void Update()
        {
            float multiplier = Time.deltaTime * _speed;
            float maxDistDelta = _crossPointsDistance * multiplier;
            _distanceLeft -= maxDistDelta;

            transform.position += _crossPointsDistanceVector * multiplier;

            if (_distanceLeft < 0)
            {
                CalculateNextPointIndex();
                CalculateCrossPoints();
            }
        }

        private void CalculateCrossPoints()
        {
            _crossPointsDistanceVector = NextPosition - PrevPosition;
            _crossPointsDistance = _crossPointsDistanceVector.magnitude;
            _distanceLeft = _crossPointsDistance;
        }

        private void CalculateNextPointIndex()
        {
            if (_curPointInd == _points.Length - 1)
                _curPointInd = 0;
            else
                ++_curPointInd;
        }

        [System.Serializable]
        private struct PathPoint
        {
            public Transform Transform;
            public bool IsStartPoint;
        }
    }
}
