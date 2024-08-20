using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDRenderer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quarterLabel;
    [SerializeField] private TextMeshProUGUI moneyLabel, salaryLabel, revenueLabel;
    [SerializeField] private Image quarterBar, projectSpawnBar;

    [SerializeField] private Button pauseButton;
    [SerializeField] private Image pauseImage;
    [SerializeField] private Sprite pauseIcon, playIcon;
    [SerializeField] private SpeedButton[] speedButtons;

    [SerializeField] private GameObject pauseIndicator;

    [System.Serializable]
    public struct SpeedButton
    {
        public Button button;
        public float speed;
        public Image outline;
    }

    private void Start()
    {
        pauseButton.onClick.AddListener(PauseClicked);
        pauseIndicator.SetActive(false);
        pauseImage.sprite = pauseIcon;

        for (int i = 0; i < speedButtons.Length; i++)
        {
            SpeedButton button = speedButtons[i];
            int ii = i;
            button.button.onClick.AddListener(() =>
            {
                SpeedClicked(ii);
            });
        }
    }

    private void PauseClicked()
    {
        bool playing = GameManager.main.TogglePause();
        pauseIndicator.SetActive(!playing);
        pauseImage.sprite = playing ? pauseIcon : playIcon;
    }

    private void SpeedClicked(int id)
    {
        for (int i = 0; i < speedButtons.Length; i++)
        {
            speedButtons[i].outline.enabled = i == id;
        }
        GameManager.main.SetSpeed(speedButtons[id].speed);
    }

    public void UpdateMoneyValues()
    {
        //todo update revenue and salary values
    }
}