using UnityEngine;

namespace VR
{
    public class CurvedUI : MonoBehaviour
    {
        private RectTransform _canvasPos;
        private bool _isOnPos;

        private void Start()
        {
            _canvasPos = GetComponent<RectTransform>();
        }

        public void Update()
        {
            if (!_isOnPos)
            {
                var pos = _canvasPos.anchoredPosition3D;
                _canvasPos.anchoredPosition3D = new Vector3(pos.x, 0.1f, 0.58f);
                _isOnPos = true;
            }
        }
    }
}