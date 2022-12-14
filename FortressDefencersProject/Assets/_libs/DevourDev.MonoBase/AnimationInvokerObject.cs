using UnityEngine;

namespace DevourDev.MonoBase
{
    [CreateAssetMenu(menuName ="DevourDev/Animation Invoker Object")]
    public class AnimationInvokerObject : ScriptableObject
    {
        [SerializeField] private bool _hasAnimation;
        [SerializeField] private AnimatorControllerParameterType _parameterType;
        [SerializeField] private string _parameterName;
        [SerializeField] private float _floatValue;
        [SerializeField] private int _intValue;
        [SerializeField] private bool _boolValue;

        public void Activate(Animator a)
        {
            if (!_hasAnimation)
                return;

            switch (_parameterType)
            {
                case AnimatorControllerParameterType.Float:
                    a.SetFloat(_parameterName, _floatValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    a.SetInteger(_parameterName, _intValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    a.SetBool(_parameterName, _boolValue);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    a.SetTrigger(_parameterName);
                    break;
                default:
                    break;
            }
        }
    }
}