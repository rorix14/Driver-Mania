using UnityEngine;

namespace VR
{
    public class UICameraFacing : MonoBehaviour
    {
        [SerializeField] private bool freezeXZRotation;
        private Camera _camera;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
        }

        private void LateUpdate()
        {
            if (!freezeXZRotation)
            {
               transform.forward = _camera.transform.forward;
            }
            else
            {
                var eulerAngles = transform.eulerAngles;
                transform.eulerAngles = new Vector3(eulerAngles.x, _camera.transform.eulerAngles.y, eulerAngles.z);
            }
        }
    }
}