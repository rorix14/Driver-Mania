using CarUtils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VR
{
    public class VRPlayerDrive : MonoBehaviour
    {
        [SerializeField] private Transform steeringWheel;
        [SerializeField] private float moveDist;
        [SerializeField] private InputActionProperty moveAction;
        [SerializeField] private InputActionProperty accelerateAction;
        [SerializeField] private InputActionProperty breakAction;
        private CarPhysics _carPhysics;
        private float _steerAngle;

        private void Awake()
        {
            _carPhysics = GetComponent<CarPhysics>();
        }

        private void FixedUpdate()
        {
            var moveAxis = moveAction.action.ReadValue<Vector2>();
            var forward = accelerateAction.action.ReadValue<float>() - breakAction.action.ReadValue<float>();
            
            _carPhysics.MoveWithCustomPhysics(forward, moveAxis.x);

            if (steeringWheel)
            {
                _steerAngle = Mathf.Lerp(_steerAngle, -90 * moveAxis.x, moveDist * Time.fixedDeltaTime);
                var eulerAngle = steeringWheel.transform.localEulerAngles;
                steeringWheel.localEulerAngles = new Vector3(eulerAngle.x, eulerAngle.y, _steerAngle);
            }
        }
    }
}