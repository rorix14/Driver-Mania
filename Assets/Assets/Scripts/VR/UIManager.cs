using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.VR
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button trackButton;
        [SerializeField] private Button carButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button confirmButton;

        [SerializeField] private RectTransform popupContainer;
        [SerializeField] private RectTransform contentContainer;
        
        private ContinuousMoveProviderBase _moveProvider;
        private ContinuousTurnProviderBase _turnProvider;
        private XRRayInteractor[] _rayInteractors;
        private XRDirectInteractor[] _directInteractors;
        
        [SerializeField] private Sprite[] trackImages;
        [SerializeField] private Sprite[] carImages;

        private void Awake()
        {
            _moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
            _turnProvider = FindObjectOfType<ContinuousTurnProviderBase>();
            _rayInteractors = FindObjectsOfType<XRRayInteractor>();
            _directInteractors = FindObjectsOfType<XRDirectInteractor>();
        }

        private void Start()
        {
            _moveProvider.enabled = false;
            _turnProvider.enabled = false;
            
            foreach (var directInteractor in _directInteractors)
            {
                directInteractor.gameObject.SetActive(false);
            }

            closeButton.onClick.AddListener(CloseMainMenu);
            trackButton.onClick.AddListener( () => ShowTrackMenu(trackImages));
            carButton.onClick.AddListener(() => ShowTrackMenu(carImages));
            
            backButton.onClick.AddListener(DisablePopupMenu);
            confirmButton.onClick.AddListener(DisablePopupMenu);
        }

        private void CloseMainMenu()
        {
            _moveProvider.enabled = true;
            _turnProvider.enabled = true;

            foreach (Transform children in transform)
            {
                children.gameObject.SetActive(false);
            }

            foreach (var rayInteractor in _rayInteractors)
            {
                rayInteractor.gameObject.SetActive(false);
            }

            foreach (var directInteractor in _directInteractors)
            {
                directInteractor.gameObject.SetActive(true);
            }
        }

        private void ShowTrackMenu(Sprite[] images)
        {
            popupContainer.gameObject.SetActive(true);
            trackButton.interactable = false;
            carButton.interactable = false;
            settingsButton.interactable = false;
            closeButton.interactable = false;
            
            var index = 0;
            foreach (Transform image in contentContainer.transform)
            {
                image.GetComponent<Image>().sprite = images[index];
                index++;
            }
        }

        private void DisablePopupMenu()
        {
            popupContainer.gameObject.SetActive(false);
            trackButton.interactable = true;
            carButton.interactable = true;
            settingsButton.interactable = true;
            closeButton.interactable = true;
        }
    }
}