using UnityEngine;

namespace FD.Utils
{
    public class GizmosMaster : MonoBehaviour
    {
        [SerializeField] private bool _onlySelected;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _sphereRadius;
        [SerializeField] private Vector3 _cubeDimentions;
        [SerializeField] private GizmosMode _mode;
        [SerializeField] private Color _color;


        private void OnDrawGizmos()
        {
            if (_onlySelected)
                return;

            Draw();
        }
        private void OnDrawGizmosSelected()
        {
            if (!_onlySelected)
                return;

            Draw();
        }


        private void Draw()
        {
            Gizmos.color = _color;
            switch (_mode)
            {
                case GizmosMode.Cube:
                    Gizmos.DrawCube(GetPosition(), _cubeDimentions);
                    break;
                case GizmosMode.WireCube:
                    Gizmos.DrawWireCube(GetPosition(), _cubeDimentions);
                    break;
                case GizmosMode.Sphere:
                    Gizmos.DrawSphere(GetPosition(), _sphereRadius);
                    break;
                case GizmosMode.WireSphere:
                    Gizmos.DrawWireSphere(GetPosition(), _sphereRadius);
                    break;
                default:
                    break;
            }
        }

        private Vector3 GetPosition()
        {
            return transform.position + _offset;
        }

        public enum GizmosMode
        {
            Cube,
            WireCube,
            Sphere,
            WireSphere
        }

    }
}
