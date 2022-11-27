using System;
using UnityEngine;

namespace FD.Utils
{
    public class RayCastTester : MonoBehaviour
    {

        [SerializeField] private KeyCode _castRayBind = KeyCode.S;
        [SerializeField] private LayerMask _hittableLayers;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        [SerializeField] private bool _enableGizmos;
        [SerializeField] private bool _transformForwardTest;
        [SerializeField] private Color _gizmosColor = Color.cyan;

        private readonly System.Diagnostics.Stopwatch _sw = new();


        private void OnDrawGizmos()
        {
            if (!_enableGizmos)
                return;

            if (_startPoint == null)
                return;


            Gizmos.color = _gizmosColor;

            if (_transformForwardTest)
            {
                Gizmos.DrawSphere(_startPoint.position, 0.5f);
                Gizmos.DrawCube(_startPoint.position + _startPoint.forward, Vector3.one * 0.5f);
            }


            if (_endPoint == null)
                return;

            Gizmos.DrawLine(_startPoint.position, _endPoint.position);

        }

        private void Update()
        {
            if (Input.GetKeyDown(_castRayBind))
                CastRay();
        }


        private void CastRay()
        {
            _sw.Restart();
            var dir = _endPoint.position - _startPoint.position;
            var ray = new Ray(_startPoint.position, dir);

            var direction = _endPoint.position - _startPoint.position;
            var hits = Physics.RaycastAll(_startPoint.position, direction, direction.magnitude, _hittableLayers);
            Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            _sw.Stop();

            Debug.Log(_sw.Elapsed.Ticks);

        }

    }
}
