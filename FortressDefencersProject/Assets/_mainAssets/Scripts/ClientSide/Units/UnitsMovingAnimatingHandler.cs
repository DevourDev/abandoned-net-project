using UnityEngine;

namespace FD.ClientSide.Units
{
    [RequireComponent(typeof(TransformInterpolator))]
    public class UnitsMovingAnimatingHandler : MonoBehaviour
    {
        [SerializeField] private DevourDev.MonoBase.AnimationInvoker[] _startMovingAnimationInvokers;
        [SerializeField] private DevourDev.MonoBase.AnimationInvoker[] _endMovingAnimationInvokers;

        [SerializeField] private TransformInterpolator _interpolator;
        [SerializeField] private Animator _animator;



        private void OnValidate()
        {
            if (_interpolator == null)
                _interpolator = GetComponent<TransformInterpolator>();
        }

        private void Start()
        {
            _interpolator.OnUnitStartMoving += UnitStartMovingHandler;
            _interpolator.OnUnitEndMoving += UnitEndMovingHandler;
        }

        private void UnitEndMovingHandler()
        {
            if (_animator == null)
                return;

            if (_endMovingAnimationInvokers == null || _endMovingAnimationInvokers.Length < 1)
                return;

            for (int i = 0; i < _endMovingAnimationInvokers.Length; i++)
            {
                DevourDev.MonoBase.AnimationInvoker invoker = _endMovingAnimationInvokers[i];
                invoker.Activate(_animator);
            }
        }

        private void UnitStartMovingHandler()
        {
            if (_animator == null)
                return;

            if (_startMovingAnimationInvokers == null || _startMovingAnimationInvokers.Length < 1)
                return;

            for (int i = 0; i < _startMovingAnimationInvokers.Length; i++)
            {
                DevourDev.MonoBase.AnimationInvoker invoker = _startMovingAnimationInvokers[i];
                invoker.Activate(_animator);
            }
        }

       
    }
}
