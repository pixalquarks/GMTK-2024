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
        SetBar(quarterBar, 0);
        projectSpawnBar.fillAmount = 0.5f;

        pauseButton.onClick.AddListener(PauseClicked);
        pauseIndicator.SetActive(false);
        pauseImage.sprite = pauseIcon;

        for (int i = 0; i < speedButtons.Length; i++)
        {
            SpeedButton button = speedButtons[i];
            int ii = i;
            button.outline.enabled = i == 0;
            button.button.onClick.AddListener(() =>
            {
                SpeedClicked(ii);
            });
        }
    }

    private void Update() {
        SetBar(quarterBar, GameManager.main.QuarterFraction);
        projectSpawnBar.fillAmount = GameManager.main.ProjectSpawnFraction;

        if (Input.GetKeyDown(KeyCode.Space)) {
            PauseClicked();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SpeedClicked(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SpeedClicked(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SpeedClicked(2);
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

    public void UpdateMoneyValue(bool animate = true)
    {
        if (animate) {
            //todo animation
            moneyLabel.text = $"${GameManager.main.Money:N0}";
        }
        else {
            moneyLabel.text = $"${GameManager.main.Money:N0}";
        }
    }

    public void UpdateMoneyDiffValues()
    {
        //update revenue and salary values
        salaryLabel.text = $"${GameManager.main.quarterlySalary:N0} <color=white>/ Q</color>";
        revenueLabel.text = $"${GameManager.main.quarterlyRevenue:N0} <color=white>/ Q</color>";
    }

    public void UpdateQuarterValue() {
        quarterLabel.text = $"Year {(GameManager.main.Quarter - 1) / 4 + 1} Quarter {(GameManager.main.Quarter - 1) % 4 + 1}";
    }

    private void SetBar(Image i, float f) {
        i.rectTransform.anchorMin = Vector2.zero;
        i.rectTransform.anchorMax = new Vector2(f, 1);
    }
}