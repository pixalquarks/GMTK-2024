using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GMTK_2024
{
    public class ChangeImageOnMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Sprite newSprite;

        private Sprite _currSprite;

        private void Awake()
        {
            _currSprite = GetComponentInParent<Image>().sprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GetComponent<Image>().sprite = newSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GetComponent<Image>().sprite = _currSprite;
        }

    }
}
