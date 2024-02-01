using CarUtils;
using DrivingData;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDriver : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _steerActon;
    private InputAction _accelerateAction;
    private InputAction _breakAction;

    private CarPhysics _carPhysics;
    private float _inputSteerValue;
    private float _inputThrottleValue;
    
    private DataGatherer _dataGatherer;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _steerActon = _playerInput.actions["Steer"];
        _accelerateAction = _playerInput.actions["Accelerate"];
        _breakAction = _playerInput.actions["Break"];

        _carPhysics = GetComponent<CarPhysics>();
        _dataGatherer = GetComponent<DataGatherer>();
    }

    private void FixedUpdate()
    {
        _inputSteerValue = _steerActon.ReadValue<float>();
        _inputThrottleValue = 1 /*_accelerateAction.ReadValue<float>() - _breakAction.ReadValue<float>()*/;
        _carPhysics.MoveWithCustomPhysics(_inputThrottleValue, _inputSteerValue);

        _dataGatherer.GatherData();
        _dataGatherer.GatherInputs(new DriveLabels(_inputSteerValue, _inputThrottleValue));
    }
}