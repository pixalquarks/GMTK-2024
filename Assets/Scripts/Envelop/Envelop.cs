using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GMTK_2024
{
    public class Envelop : MonoBehaviour
    {
        [SerializeField] private Light2D light;
        [SerializeField] private EnvelopCanvas envelopCanvas;
        [SerializeField] private string message;
        [SerializeField] private float maxIntensity = 10;
        [SerializeField] private float speed = 1;


        private void Update()
        {
            light.intensity = Mathf.PingPong(Time.time * speed, maxIntensity);
        }

        private void OnMouseDown()
        {
            Debug.Log("Clicked on Message ");
            envelopCanvas.ShowMessage(message);
            Destroy(gameObject);
        }
    }
}
