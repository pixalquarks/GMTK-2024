using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeRenderer : MonoBehaviour
{
    [SerializeField] private Employee employee;
    public Canvas canvas;

    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image roleIcon;
    [SerializeField] private Image typeIcon;

    [SerializeField] private GameObject levelupIndicator;

    [SerializeField] private Image torso;
    [SerializeField] private Image[] heads = { };

    [SerializeField] private Material hueMaterial;

    private Material headMat, torsoMat;

    private void Start()
    {
        levelLabel.text = $"LV{employee.Level}";
        levelLabel.color = UIThemeManager.main.levelColors[employee.Level - 1];

        roleIcon.sprite = UIThemeManager.main.GetRoleIcon(employee.role);
        typeIcon.sprite = employee.type.icon;
        typeIcon.color = employee.type.color;

        int h = Random.Range(0, heads.Length);
        for (int i = 0; i < heads.Length; i++)
        {
            heads[i].enabled = h == i;
        }

        headMat = new Material(hueMaterial);
        torsoMat = new Material(hueMaterial);

        heads[h].material = headMat;
        torso.material = torsoMat;
        headMat.SetFloat("_HueChange", Random.Range(0, 360f));
        torsoMat.SetFloat("_HueChange", Random.Range(0, 360f));

        CheckLevelIndicator();
        employee.onLevelUp.AddListener(UpdateLevel);
        employee.onStatsChange.AddListener(CheckLevelIndicator);
    }

    private void UpdateLevel()
    {
        levelLabel.text = $"LV{employee.Level}";
        levelLabel.color = UIThemeManager.main.levelColors[employee.Level - 1];

        CheckLevelIndicator();
    }

    private void CheckLevelIndicator() {
        levelupIndicator.SetActive(employee.HasUnspentSkillPoints());
    }

    private void OnDestroy()
    {
        Destroy(headMat);
        Destroy(torsoMat);
    }
}
