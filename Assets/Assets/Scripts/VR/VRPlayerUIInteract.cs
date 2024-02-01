using UnityEngine;

namespace VR
{
    public class VRPlayerUIInteract : MonoBehaviour
    {
        [SerializeField] private LayerMask vruiLayerMask;
        [SerializeField] private float rayDistance;
        [SerializeField] private float testSize;
        private Camera _camera;
        private Vector3 _hitPoint;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
        }

        private void LateUpdate()
        {
            var cameraTransform = _camera.transform;
            var cameraRay = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.SphereCast(cameraRay, testSize, out var hit, rayDistance, vruiLayerMask))
            {
                _hitPoint = hit.point;
                var interactable = hit.collider.gameObject.GetComponent<UIOpenCarDor>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            var cameraTransform = _camera.transform;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                _hitPoint != Vector3.zero
                    ? _hitPoint
                    : cameraTransform.position + cameraTransform.forward * rayDistance, testSize);

            _hitPoint = Vector3.zero;
        }
    }
}