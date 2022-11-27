using UnityEngine;

namespace FD.Utils
{
    public class RotationRepeater : MonoBehaviour
    {
        [SerializeField] private Transform _source;
        [SerializeField] private bool _setMainCamNow;
        [SerializeField] private bool _setMainCamOnAwake;

        [SerializeField] private bool _updateEveryFrame = true;
        [SerializeField] private bool _initialRepeatingState = true;

        [SerializeField] private bool _ignoreX;
        [SerializeField] private bool _ignoreY;
        [SerializeField] private bool _ignoreZ;

        private Transform _currentSource;


        public bool Repeating { get; set; }



        private void OnValidate()
        {
            if (_setMainCamNow)
            {
                _setMainCamNow = false;
                _source = Camera.main.transform;
            }
        }

        private void Awake()
        {
            Repeating = _initialRepeatingState;
            if (_setMainCamOnAwake)
            {
                _source = Camera.main.transform;
            }

            SetSource(_source);
        }

        private void LateUpdate()
        {
            if (_updateEveryFrame)
                RepeatRotation();
        }


        public void SetSource(Transform t)
        {
            _currentSource = t;
        }


        public void RepeatManually()
        {
            RepeatRotation();
        }

        public void RepeatManually(bool lockX, bool lockY, bool lockZ)
        {
            RepeatRotation(lockX, lockY, lockZ);
        }


        private void RepeatRotation()
        {
            if (Repeating)
            {
                Vector3 qr = _currentSource.rotation.eulerAngles;

                if (_ignoreX)
                    qr.x = transform.rotation.x;
                if (_ignoreY)
                    qr.y = transform.rotation.y;
                if (_ignoreZ)
                    qr.z = transform.rotation.z;

                transform.rotation = Quaternion.Euler(qr);
            }
        }

        private void RepeatRotation(bool lockX, bool lockY, bool lockZ)
        {
            if (Repeating)
            {
                Vector3 qr = _currentSource.rotation.eulerAngles;

                if (lockX)
                    qr.x = transform.rotation.x;
                if (lockY)
                    qr.y = transform.rotation.y;
                if (lockZ)
                    qr.z = transform.rotation.z;

                transform.rotation = Quaternion.Euler(qr);
            }
        }
    }
}
