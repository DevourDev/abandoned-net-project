using UnityEngine;

namespace DevourDev.MonoBase
{
    [System.Obsolete("Нужно добавить булы на форсированный поворот (для объектов без рендерера)")]
    public class FaceRotator : MonoBehaviour
    {

        [SerializeField] private bool _rotateAll;
        [SerializeField] private bool _rotateX;
        [SerializeField] private bool _rotateY;
        [SerializeField] private bool _rotateZ;

        [SerializeField] private Transform _target;
        [SerializeField] private bool _targetIsMainCamera;
        [SerializeField] private bool _adjustOnStart;
        [SerializeField] private bool _adjustEveryUpdate;

        [SerializeField] private Renderer _renderer;
        [SerializeField] private bool _customRenderer;



        public Transform Target { get => _target; set => _target = value; }



        public void Init(bool rotateAll, bool rotateX, bool rotateY, bool rotateZ, Transform target, bool targetIsMainCamera, bool adjustOnStart, bool adjustEveryUpdate)
        {
            _rotateAll = rotateAll;
            _rotateX = rotateX;
            _rotateY = rotateY;
            _rotateZ = rotateZ;
            _target = target;
            _targetIsMainCamera = targetIsMainCamera;
            _adjustOnStart = adjustOnStart;
            _adjustEveryUpdate = adjustEveryUpdate;
        }


        private void Start()
        {
            if (_targetIsMainCamera)
            {
                Target = Camera.main.transform;
            }

            if (_adjustEveryUpdate)
            {
                if (!_customRenderer)
                    _renderer = GetComponent<Renderer>();

                if (_renderer == null)
                    Debug.LogError("No Renderer found");
            }

            if (_adjustOnStart && _target != null)
            {
                AdjustRotation(_target);
            }

        }

        private void LateUpdate()
        {
            if (_adjustEveryUpdate)
            {
                if (_renderer.isVisible)
                {
                    AdjustRotation(Target);
                }
            }
        }


        public void AdjustRotation(Transform tr)
        {
            if (_rotateAll)
            {
                transform.rotation = tr.rotation;
            }
            else
            {
                Vector3 newRot = tr.rotation.eulerAngles;
                if (_rotateX)
                    newRot.x = tr.rotation.eulerAngles.x;
                if (_rotateY)
                    newRot.y = tr.rotation.eulerAngles.y;
                if (_rotateZ)
                    newRot.z = tr.rotation.eulerAngles.z;
                transform.rotation = Quaternion.Euler(newRot);
            }
        }
    }
}