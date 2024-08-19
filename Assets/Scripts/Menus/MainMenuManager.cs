using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CozyChaosSpring2024
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private RectTransform optionsPanel;
        private UIAnimation[] _uiAnimationsElements;
        public const float UIElemAnimDuration = 0.6f;

        private const float OptionsPanelAnimDuration = 0.6f;

        private void Awake()
        {
            _uiAnimationsElements = FindObjectsByType<UIAnimation>(FindObjectsSortMode.None);
        }

        public void OnPlay()
        {
            var sceneTransition = FindObjectOfType<SceneTransition>();
            sceneTransition.PlayTransition(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OnOptionsClicked()
        {
            foreach (var uiElem in _uiAnimationsElements)
            {
                uiElem.Shrink();
            }

            Invoke(nameof(SlideIn), UIElemAnimDuration);
        }

        private void SlideIn() => optionsPanel.DOAnchorPos(new Vector2(960, 0), OptionsPanelAnimDuration);

        public void OnCreditsClicked()
        {
            SceneManager.LoadScene(4);
        }

        public void OnOptionsMenuClosed()
        {
            SlideOut();
            Invoke(nameof(GrowElements), UIElemAnimDuration);
        }

        private void GrowElements() => _uiAnimationsElements.ToList().ForEach(uiElem => uiElem.Grow());

        private void SlideOut() => optionsPanel.DOAnchorPos(new Vector2(-1000, 0), OptionsPanelAnimDuration);
    }
}
