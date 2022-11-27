using UnityEngine;
using System.Runtime.CompilerServices;

namespace FD.ClientSide.Units
{
    public class TransformInterpolator : MonoBehaviour //deltaless
    {
        public event System.Action OnUnitStartMoving;
        public event System.Action OnUnitEndMoving;

        [SerializeField] private int _serverTickrate = 10;

        private bool _stopOnEndpoint = true;

        private bool _interpolatingPosition;
        private Vector3 _nextTurnPos;
        private float _crossTicksDistance;
        private Vector3 _crossTicksDistanceVector;
        private float _distanceLeft;

        private bool _interpolatingRotation;
        private float _nextTurnYRot;
        private float _crossTicksRot;
        private float _yRotationLeft;

        private bool _interpolating;


        public void SetNextTurnTransform(Vector3 pos, float yRot)
        {
            _interpolatingPosition = true;
            _interpolatingRotation = true;

            _nextTurnPos = pos;
            _crossTicksDistanceVector = _nextTurnPos - transform.position;
            _crossTicksDistance = _crossTicksDistanceVector.magnitude;
            _distanceLeft = _crossTicksDistance;

            _nextTurnYRot = yRot;
            _crossTicksRot = _nextTurnYRot - transform.rotation.eulerAngles.y;
            _yRotationLeft = _crossTicksRot;

            StartInterpolating();
        }
        public void StayInPlace()
        {
            StopInterpolating();
        }



        private void Update()
        {
            Interpolate();
        }

        private void StartInterpolating()
        {
            if (_interpolating)
                return;

            _interpolating = true;
            OnUnitStartMoving?.Invoke();
        }
        private void StopInterpolating()
        {
            if (!_interpolating)
                return;

            _interpolating = false;
            OnUnitEndMoving?.Invoke();
        }


        //private System.Diagnostics.Stopwatch _sw = new();
        //private double[] _elapseds = new double[100];
        //private int _currentIndex = 0;
        private void Interpolate()
        {
            if (!_interpolating || (!_interpolatingPosition && !_interpolatingRotation))
                return;

            //_sw.Restart();
            float multiplier = Time.deltaTime * _serverTickrate;
            Vector3 pos = CalculateNextFramePosition(multiplier);
            Quaternion rot = CalculateNextFrameRotation(multiplier);
            //_sw.Stop();

            //var el = _sw.Elapsed.TotalMilliseconds;

            //if (_currentIndex == 100)
            //{

            //    _currentIndex = 0;
            //    var avg = _elapseds.Average();
            //    Debug.Log(avg);
            //}
            //else
            //{
            //    _elapseds[_currentIndex] = el;
            //    _currentIndex++;
            //}

            transform.SetPositionAndRotation(pos, rot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Quaternion CalculateNextFrameRotation(float multiplier)
        {
            var deltaRot = _crossTicksRot * multiplier;
            _yRotationLeft -= System.Math.Abs(deltaRot);
            var nextRot = transform.rotation.eulerAngles;

            if (_yRotationLeft < 0)
            {
                _interpolatingRotation = false;
                nextRot.y = _nextTurnYRot;
            }
            else
            {
                nextRot.y += deltaRot;
            }
            return Quaternion.Euler(nextRot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 CalculateNextFramePosition(float multiplier)
        {
            if (!_interpolatingPosition)
                return _nextTurnPos;

            float maxDistanceDelta = _crossTicksDistance * multiplier;
            _distanceLeft -= maxDistanceDelta;

            if (_stopOnEndpoint && _distanceLeft < 0)
            {
                _interpolatingPosition = false;
                return _nextTurnPos;
            }

            return transform.position + _crossTicksDistanceVector * multiplier;
        }
    }

    public class TransformInterpolator_2 : MonoBehaviour
    {
        [SerializeField] private int _serverTickrate = 10;

        private bool _transformSet;
        private bool _stayedLastTurn;
        private bool _stopOnReachingEP;

        private Vector3 _pos;
        private float _yRot;


        private Vector3 _posDelta;
        private float _yRotDelta;

        private float _moveDistanceLeft;
        private float _rotLeft;

        private bool _interpolatingPosition;
        private bool _interpolatingRotation;


        protected bool Interpolating => _interpolatingPosition || _interpolatingRotation;


        private void Update()
        {
            InterpolateTransform();
        }


        public void SetNextTurnPosition(Vector3 pos)
        {
            _pos = pos;
            _moveDistanceLeft = Vector3.Distance(transform.position, pos);
            _transformSet = true;
            _interpolatingPosition = true;
            _posDelta = _pos - transform.position;
            _stayedLastTurn = false;
        }

        public void SetNextTurnRotation(float yRot)
        {
            _yRot = yRot;
            _rotLeft = Mathf.Abs(yRot - transform.rotation.eulerAngles.y);
            _transformSet = true;
            _interpolatingRotation = true;
            _yRotDelta = _yRot - transform.rotation.eulerAngles.y;
            _stayedLastTurn = false;
        }

        public void StayInPlace()
        {
            if (_stayedLastTurn || !_transformSet)
                return;

            _stopOnReachingEP = true;
            SetNextTurnPosition(_pos);
            SetNextTurnRotation(_yRot);
            _stayedLastTurn = true;
        }


        private void InterpolateTransform() //na$rano :_(
        {
            if (!Interpolating)
                return;

            float multiplier = Time.deltaTime * _serverTickrate;
            InterpolatePosition(multiplier);
            InterpolateRotation(multiplier);
        }

        private void InterpolatePosition(float multiplier)
        {
            if (!_interpolatingPosition)
                return;

            Vector3 movement = _posDelta * multiplier;
            var magnitude = movement.magnitude;
            _moveDistanceLeft -= magnitude;

            if (_stopOnReachingEP)
            {
                if (_moveDistanceLeft <= magnitude)
                {
                    transform.position = _pos;
                    _interpolatingPosition = false;
                    return;
                }
            }

            transform.position = transform.position + movement;
        }
        private void InterpolateRotation(float multiplier)
        {
            if (!_interpolatingRotation)
                return;

            float yRotation = _yRotDelta * multiplier;
            var absRot = Mathf.Abs(yRotation);

            if (_stopOnReachingEP)
            {
                if (_rotLeft <= absRot)
                {
                    //var newRot = transform.rotation.eulerAngles;
                    //newRot.y = _yRot;
                    //transform.rotation = Quaternion.Euler(newRot);
                    transform.Rotate(0, _yRotDelta > 0 ? _rotLeft : -_rotLeft, 0);
                    _interpolatingRotation = false;
                    return;
                }
            }

            transform.Rotate(0, yRotation, 0);
        }




    }
}
