using UnityEngine;
using UnityEngine.InputSystem;

namespace VR
{
    public class HandAnimator : MonoBehaviour
    {
        [SerializeField] private InputActionProperty grabAction;
        [SerializeField] private string grabAnimationName;
        [SerializeField] private InputActionProperty activateAction;
        [SerializeField] private string activateAnimationName;
        [SerializeField] private Animator animator;

        private bool _grabbing;
        private bool _activating;

        private void Update()
        {
            if (!animator) return;

            var grabButton = grabAction.action.ReadValue<float>();
            var activateButton = activateAction.action.ReadValue<float>();

            if (grabButton > 0)
            {
                if (!_grabbing)
                {
                    _grabbing = true;
                    animator.SetBool(grabAnimationName, _grabbing);
                }
            }
            else if (_grabbing)
            {
                _grabbing = false;
                animator.SetBool(grabAnimationName, _grabbing);
            }

            if (activateButton > 0)
            {
                if (!_activating)
                {
                    _activating = true;
                    animator.SetBool(activateAnimationName, _activating);
                }
            }
            else if (_activating)
            {
                _activating = false;
                animator.SetBool(activateAnimationName, _activating);
            }
        }
    }
}