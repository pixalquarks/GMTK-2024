using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK_2024
{
    public class CanvasEvent : MonoBehaviour
    {
        public void OnAnimationComplete()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
