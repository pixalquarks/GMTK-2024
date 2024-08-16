using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace CozyChaosSpring2024
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextSizeManager : MonoBehaviour
    {
        public enum TextSizes
        {
            regular,
            large,
            extraLarge
        }
        public static TextSizes size = TextSizes.regular;
        TextMeshProUGUI text;
        [SerializeField] HorizontalSelector selector;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            OnSizeChange();

            if (selector != null)
                selector.onOptionsChange += OnSizeChange;

            if (selector == null)
            {
                int tempInt = PlayerPrefs.GetInt("TextSize", 0);
                if(tempInt == 0)
                    size = TextSizes.regular;

                if(tempInt == 1)
                    size = TextSizes.large;

                if(tempInt == 2)
                    size = TextSizes.extraLarge;
            }
        }

        public void OnSizeChange()
        {
            if(SceneManager.GetSceneByName("MainBedroom") == SceneManager.GetActiveScene())
            {
                switch (size)
                {
                    case TextSizes.regular:
                        text.fontSize = 16;
                        break;
                    case TextSizes.large:
                        text.fontSize = 18;
                        break;
                    case TextSizes.extraLarge:
                        text.fontSize = 20;
                        break;
                }
            }
            if (SceneManager.GetSceneByName("EndScene") == SceneManager.GetActiveScene())
            {
                switch (size)
                {
                    case TextSizes.regular:
                        text.fontSize = 40;
                        break;
                    case TextSizes.large:
                        text.fontSize = 44;
                        break;
                    case TextSizes.extraLarge:
                        text.fontSize = 48;
                        break;
                }
            }
            else
            {
                if (SceneManager.GetSceneByName("EndScene") == SceneManager.GetActiveScene() && SceneManager.GetSceneByName("MainBedroom") == SceneManager.GetActiveScene())
                {
                    switch (size)
                    {
                        case TextSizes.regular:
                            text.fontSize = 30;
                            break;
                        case TextSizes.large:
                            text.fontSize = 34;
                            break;
                        case TextSizes.extraLarge:
                            text.fontSize = 42;
                            break;
                    }
                }
            }
        }
    }
}
