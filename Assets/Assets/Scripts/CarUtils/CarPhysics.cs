using UnityEngine;

namespace CarUtils
{
    public class CarPhysics : MonoBehaviour
    {
        [SerializeField] private float mass;
        [SerializeField] private float maxDrivingForce;
        [SerializeField] private float minimumTurningRadius;

        [SerializeField] private float dragCoefficient;
        [SerializeField] private float rollingCoefficient;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundRayDistance;

        [SerializeField] private LayerMask wallLayer;

        private Vector3 _velocity;
        private Vector3 _acceleration;

        public Vector3 Velocity => _velocity;
        public Vector3 Acceleration => _acceleration;
        public LayerMask GroundLayer => groundLayer;
        public float GroundRayDistance => groundRayDistance;

        public void MoveWithCustomPhysics(float forwardInput, float steerInput)
        {
            var forward = transform.forward;

            var isOnGround = Physics.Raycast(transform.position, -1 * transform.up,
                groundRayDistance, groundLayer);

            var rotationAngle = isOnGround
                ? Vector3.Dot(forward, _velocity) * Time.fixedDeltaTime / minimumTurningRadius * steerInput
                : 0.0f;

            transform.Rotate(transform.up, Mathf.Rad2Deg * rotationAngle);
            _velocity = forward * Vector3.Dot(forward, _velocity);

            var force = isOnGround ? forward * (maxDrivingForce * forwardInput) :  Vector3.zero;

            var airForceResistance = -_velocity.normalized * (_velocity.sqrMagnitude * dragCoefficient);
            var rollingResistance =
                isOnGround ? _velocity.normalized * (rollingCoefficient * mass * Physics.gravity.y) : Vector3.zero;
            _acceleration = (force + airForceResistance + rollingResistance) / mass;

            _velocity += _acceleration * Time.fixedDeltaTime;
            transform.position += _velocity * Time.fixedDeltaTime;
        }

        public void ResetCar()
        {
            _velocity = Vector3.zero;
            _acceleration = Vector3.zero;
            var rb = GetComponent<Rigidbody>();
            if(rb) rb.velocity = Vector3.zero;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == wallLayer)
            {
                _velocity = Vector3.zero;
            }
        }
        
        // use to calculate velocity rotation if we want the break to also turn back
        // Vector3 fwd = Quaternion.Euler(0, Mathf.Rad2Deg * rotationAngle, 0) * Vector3.forward;
        // var up = transform.up;
        // Vector4 right = Vector3.Cross(up, fwd);
        // fwd = Vector3.Cross(right, up);
        //
        // Quaternion rotationDelta = Quaternion.LookRotation(fwd, up);
        // _velocity = rotationDelta * _velocity;

      /*  private void MoveWithInGamePhysics()
        {
            _velocity = _rb.velocity;

            var forward = transform.forward;
            float rotationAngle = Vector3.Dot(forward, _velocity) * Time.fixedDeltaTime / minimumTurningRadius *
                                  _steerActon.ReadValue<float>();

            transform.Rotate(transform.up, Mathf.Rad2Deg * rotationAngle);
            _velocity = forward * Vector3.Dot(forward, _velocity);

            Vector3 force = transform.forward * maxDrivingForce *
                            (_accelerateAction.ReadValue<float>() - _breakAction.ReadValue<float>());

            // Vector3 airForceResistance = -_velocity.normalized * _velocity.sqrMagnitude * dragCoefficient;
            // Vector3 rollingResistance = _velocity.normalized * rollingCoefficient * mass * Physics.gravity.y;
            Vector3 acceleration = force / mass;
            _velocity += acceleration * Time.fixedDeltaTime;
            // _rb.velocity = _velocity;
            _rb.AddForce(_velocity * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }*/
    }
}