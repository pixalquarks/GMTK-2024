using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GMTK_2024
{
    public class EnvelopCanvas : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform paper;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private TMP_Text message;

        private bool _isActive = false;

        private void Awake()
        {
            paper.gameObject.SetActive(false);
        }


        private bool _clickedOnPaper = false;
        private void Update()
        {
            // if (_isActive && Input.GetMouseButtonDown(0) && !_clickedOnPaper)
            // {
            //     Debug.Log("Click detected outside");
            //     // Debug.Log(eventSystem.IsPointerOverGameObject());
            //     // Debug.Log(EventSystem.current.currentSelectedGameObject);
            //     // if (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != paper.gameObject)
            //     // {
            //     //     _isActive = false;
            //     //     paper.gameObject.SetActive(false);
            //     // }
            // }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isActive = false;
                paper.gameObject.SetActive(false);
            }
        }

        public void ShowMessage(string message)
        {
            this.message.text = message;
            _isActive = true;
            Debug.Log("Showing Message");
            paper.gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _clickedOnPaper = true;
            Debug.Log("Click detected");
        }
    }
}
