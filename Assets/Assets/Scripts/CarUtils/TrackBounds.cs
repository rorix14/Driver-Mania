using DrivingData;
using UnityEngine;

namespace CarUtils
{
    public class TrackBounds : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var car = other.GetComponent<DataGatherer>();
            if (!car) return;

            var carTransform = car.transform;
            carTransform.position = car.CurrentCheckPoint.transform.position;
            carTransform.forward = car.CurrentCheckPoint.GetCornerDir.normalized;

            var catPhysics = carTransform.GetComponent<CarPhysics>();
            if (catPhysics) catPhysics.ResetCar();
        }
    }
}
