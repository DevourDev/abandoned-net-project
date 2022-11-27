using UnityEngine;

namespace FD.Test
{
    [RequireComponent(typeof(PseudoGameSide_test))]
    public class PseudoGameSideSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPositions;
        [SerializeField] private Units.UnitObject _unitObject;
        [SerializeField] private KeyCode _spawnKeyBind = KeyCode.G;

        private PseudoGameSide_test _pgs;
        private Global.GameManager _gm;

        private void Start()
        {
            _gm = Global.GameManager.Instance;
            if (!TryGetComponent(out _pgs))
            {
                Debug.Log($"Unable to find {nameof(PseudoGameSide_test)} component.");
            }
        }

        private void Update()
        {
            if (_pgs == null)
                return;

            if (Input.GetKeyDown(_spawnKeyBind))
                SpawnUnits();
        }

        private void SpawnUnits()
        {
            foreach (var tr in _spawnPositions)
            {
                _gm.SpawnUnit_for_pseudo(_unitObject, _pgs.Side, tr.position);
            }
        }
    }
}
