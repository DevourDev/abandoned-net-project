using UnityEngine;

namespace FD.Utils
{
    public class CanvasCameraSetter : MonoBehaviour
    {
        [SerializeField] private Camera _specialCam;
        [SerializeField] private bool _setMain;
        [SerializeField] private bool _setMainOnAwake;


        private void OnValidate()
        {
            if (_setMain)
            {
                _setMain = false;
                _specialCam = Camera.main;
            }
        }


        private void Awake()
        {
            if (_setMainOnAwake)
                _specialCam = Camera.main;

            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = _specialCam;
        }


    }
}
