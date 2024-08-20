using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace GMTK_2024
{
    public class MainMenuCanvasController : MonoBehaviour
    {
        [SerializeField] private RectTransform mainMenuCanvas;
        [SerializeField] private RectTransform optionsCanvas;
        [SerializeField] private Animator canvasAnimator;
        [SerializeField] private Volume volume;

        private int burnTrigger = Animator.StringToHash("burn");
        private int zoomInTrigger = Animator.StringToHash("zoomIn");
        private int zoomOutTrigger = Animator.StringToHash("zoomOut");


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && optionsCanvas.gameObject.activeSelf)
            {
                OnOptionsClosed();
            }
        }

        public void OnOptionsClicked()
        {
            mainMenuCanvas.gameObject.SetActive(false);
            optionsCanvas.gameObject.SetActive(true);
            canvasAnimator.SetTrigger(zoomInTrigger);
        }

        public void OnOptionsClosed()
        {
            mainMenuCanvas.gameObject.SetActive(true);
            optionsCanvas.gameObject.SetActive(false);
            canvasAnimator.SetTrigger(zoomOutTrigger);
        }

        public void OnPlayClicked()
        {
            mainMenuCanvas.gameObject.SetActive(false);
            optionsCanvas.gameObject.SetActive(false);
            canvasAnimator.SetTrigger(burnTrigger);
        }

    }
}
