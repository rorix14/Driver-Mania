using UnityEngine;

namespace VR
{
    public class VRCameraAdjust : MonoBehaviour
    {
        [SerializeField] private Transform cameraOffset;
        [SerializeField] private float cameraHeightAdjust;

        private void Start()
        {
            cameraOffset.position += new Vector3(0, cameraHeightAdjust, 0);
        }
    }
}