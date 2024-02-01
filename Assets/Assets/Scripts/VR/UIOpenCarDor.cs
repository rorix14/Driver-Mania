using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace VR
{
    public class UIOpenCarDor : XRBaseInteractable, IVRInteractable
    {
        [SerializeField] private Sprite defaultImage;
        [SerializeField] private Sprite targetedImage;
        private Image _image;
        private bool _isInteracting;
        
        private void Start()
        {
            activated.AddListener(ActivateUI);
            _image = GetComponentInChildren<Image>();
            _image.sprite = defaultImage;
            _image.rectTransform.sizeDelta = new Vector2 (5, 5);
            
            // selectEntered.AddListener(arg0 => print("selectEntered"));
            // selectExited.AddListener(arg0 => print("selectExited"));
            // hoverEntered.AddListener(arg0 => print("hoverEntered"));
            // hoverExited.AddListener(arg0 => print("hoverExited"));
        }

        private void LateUpdate()
        {
            if (_isInteracting)
            {
                if (_image.sprite == defaultImage)
                {
                    _image.sprite = targetedImage;
                    _image.rectTransform.sizeDelta = new Vector2 (20, 20);
                }
                
                _isInteracting = false;
            }
            else
            {
                if (_image.sprite == targetedImage)
                {
                    _image.sprite = defaultImage;
                    _image.rectTransform.sizeDelta = new Vector2 (5, 5);
                }
            }
        }

        public void Interact()
        {
            _isInteracting = true;
        }
        

        private void ActivateUI(ActivateEventArgs args)
        {
            var sceneTransition = FindObjectOfType<SceneChanger>(); 
            sceneTransition.StartTransition("VR Racing");
        }
    }
}