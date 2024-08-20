using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK_2024
{
    public class BackgroundChanger : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Sprite basementBg;
        [SerializeField] private Sprite cityBg;
        [SerializeField] private Sprite spaceBg;
        [SerializeField] private int cityThreshold = 500000;
        [SerializeField] private int spaceThreshold = 200000;

        private void Update()
        {
            if (GameManager.main.Money >= cityThreshold)
            {
                background.sprite = cityBg;
            }
            else if (GameManager.main.Money >= spaceThreshold)
            {
                background.sprite = spaceBg;
            }
        }
    }
}
