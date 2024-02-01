using System.Collections;
using System.Collections.Generic;
using DrivingData;
using UnityEngine;

namespace CarUtils
{
    public class CarDebugger : MonoBehaviour
    {
        [SerializeField] private bool active;

        private CarPhysics _carPhysics;
        private DataGatherer _dataGatherer;
        private List<Vector3> _carPositions;
        private WaitForSeconds _coroutineWait;

        private void Awake()
        {
            _carPhysics = GetComponent<CarPhysics>();
            _dataGatherer = GetComponent<DataGatherer>();
            _carPositions = new List<Vector3>();
            _coroutineWait = new WaitForSeconds(0.5f);
            
            StartCoroutine(AddCarPositions());
        }
        
        private IEnumerator AddCarPositions()
        {
            _carPositions.Add(transform.position);
            yield return _coroutineWait;
            StartCoroutine(AddCarPositions());
        }

        private void OnDrawGizmos()
        {
            if (!active || !Application.isPlaying) return;

            var carTransform = transform;
            
            //print("vel " + _carPhysics.Velocity.y + ", acc " + _carPhysics.Acceleration.y);
            // var vel = _carPhysics.Velocity;
            // vel.y = 0;
            // var acc = _carPhysics.Acceleration;
            // acc.y = 0;
            // // Gizmos.color = Color.yellow;
            // Gizmos.DrawRay(carTransform.position, vel.normalized * _carPhysics.Velocity.magnitude);
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawRay(carTransform.position, acc.normalized * _carPhysics.Acceleration.magnitude);
            // Gizmos.color = Color.red;
            // Gizmos.DrawRay(carTransform.position, -1 * carTransform.up * _carPhysics.GroundRayDistance);

            if (_dataGatherer == null)
                return;
            
            foreach (var direction in _dataGatherer.ConvertedDirections)
            {
                Gizmos.color = Physics.Raycast(carTransform.position, direction, out RaycastHit hit,
                    _dataGatherer.RayDistance, _dataGatherer.WallMask)
                    ? Color.red
                    : Color.green;
            
                Gizmos.DrawRay(carTransform.position, direction * hit.distance);
            }

            Gizmos.color = Color.magenta;
            for (int i = 0; i < _carPositions.Count; i++)
            {
                if (i < _carPositions.Count - 1)
                {
                    Gizmos.DrawLine(_carPositions[i], _carPositions[i + 1]);
                }
            }
        }
    }
}