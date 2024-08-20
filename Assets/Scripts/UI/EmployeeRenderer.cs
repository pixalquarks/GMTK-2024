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

    [SerializeField] private Image torso;
    [SerializeField] private Image[] heads = { };

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

        employee.onLevelUp.AddListener(UpdateLevel);
    }

    private void UpdateLevel()
    {
        levelLabel.text = $"LV{employee.Level}";
        levelLabel.color = UIThemeManager.main.levelColors[employee.Level - 1];
    }
}
