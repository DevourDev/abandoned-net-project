using UnityEngine;

namespace FD.ClientSide.Units
{
    public class SetPositionAndRotationTester : MonoBehaviour
    {
        [SerializeField] private float _crossTicksRot;
        [SerializeField] private float _delta;
        [SerializeField] private Quaternion _nextRot;

        private void Update()
        {
            _delta = _crossTicksRot * Time.deltaTime * 10;
            Vector3 nr = transform.rotation.eulerAngles;
            nr.y += _delta;
            _nextRot = Quaternion.Euler(nr);

            transform.SetPositionAndRotation(transform.position, _nextRot);
        }
    }
}
