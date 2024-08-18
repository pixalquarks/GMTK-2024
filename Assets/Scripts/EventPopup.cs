using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2024
{
    public class EventPopup : MonoBehaviour
    {
        [SerializeField] private float maxRotationDelta = 20f;
        [SerializeField] private float minRotationDelta = 5f;

        [SerializeField] private RectTransform eventPanel;
        [SerializeField] private RectTransform newsSplash;

        private void Awake()
        {
            HideEventSplash();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HideEventSplash();
                ShowEventSplash();
            }
        }

        private void ShowEventSplash()
        {
            eventPanel.gameObject.SetActive(true);
            newsSplash.gameObject.SetActive(true);
            newsSplash.rotation = GetRandomRotation();
        }

        private void HideEventSplash()
        {
            eventPanel.gameObject.SetActive(false);
            newsSplash.gameObject.SetActive(false);
        }

        private Quaternion GetRandomRotation()
        {
            return Quaternion.Euler(0f, 0f, (Random.value > 0.5 ? 1 : -1) * Random.Range(-maxRotationDelta, maxRotationDelta));
        }

    }
}
