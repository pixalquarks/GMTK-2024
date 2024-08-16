using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CozyChaosSpring2024
{
    public class SceneTransition : MonoBehaviour
    {

        [SerializeField] private GameObject bgPanel;
        [SerializeField] private GameObject textPanel;
        [SerializeField] private GameObject instructionPanel;
        [SerializeField] private float textAppearDelay = 1.0f;

        private bool _canLoadMainGame;

        private int nextSceneIndex;


        public void PlayTransition(int nextSceneIndex)
        {
            this.nextSceneIndex = nextSceneIndex;
            StartCoroutine(PlayEffect());
        }

        private IEnumerator PlayEffect()
        {
            bgPanel.SetActive(true);
            yield return new WaitForSeconds(textAppearDelay);
            textPanel.SetActive(true);
            yield return new WaitForSeconds(textAppearDelay);
            _canLoadMainGame = true;
            instructionPanel.SetActive(true);
        }

        private void Update()
        {
            if (_canLoadMainGame && Input.GetMouseButtonDown(0)) LoadNextScene();
        }


        private void LoadNextScene()
        {
            _canLoadMainGame = false;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
