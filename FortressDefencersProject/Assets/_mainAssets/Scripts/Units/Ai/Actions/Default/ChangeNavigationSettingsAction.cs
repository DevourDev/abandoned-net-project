using DevourDev.MonoBase.AbilitiesSystem;
using FD.Global.Rules;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Navigation/Change NMA Settings")]
    public class ChangeNavigationSettingsAction : UnitAction
    {
        [SerializeField] private float _moveSpeedMultiplier = 1f;
        [SerializeField] private float _moveSpeedFlat;
        [SerializeField] private float _rotationSpeedMultiplier = 1f;
        [SerializeField] private float _rotationSpeedFlat;
        [SerializeField] private float _accelerationMultiplier = 1f;
        [SerializeField] private float _accelerationFlat;

        public override void Act(UnitAi ai)
        {
            var cs = ai.ServerSideUnit.Reference.CommonStats;
            float ms = cs.MoveSpeed * _moveSpeedMultiplier + _moveSpeedFlat;
            float rs = cs.RotationSpeed * _rotationSpeedMultiplier + _rotationSpeedFlat;
            float ac = cs.Acceleration * _accelerationMultiplier + _accelerationFlat;

            ai.ServerSideUnit.NavMeshAgent.speed = ms;
            ai.ServerSideUnit.NavMeshAgent.angularSpeed = rs;
            ai.ServerSideUnit.NavMeshAgent.acceleration = ac;
        }
    }
}
