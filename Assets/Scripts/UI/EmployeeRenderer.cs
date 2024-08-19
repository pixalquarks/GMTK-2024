using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeRenderer : MonoBehaviour
{
    [SerializeField] private Employee employee;

    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Image roleIcon;
    [SerializeField] private Image typeIcon;

    [SerializeField] private Color[] levelColors = { };
    [SerializeField] private Sprite programmerIcon, artistIcon;

    private void Start()
    {
        levelLabel.text = $"LV{employee.Level}";
        levelLabel.color = levelColors[employee.Level - 1];

        roleIcon.sprite = employee.role == Employee.EmployeeRole.Programmer ? programmerIcon : artistIcon;
        typeIcon.sprite = employee.type.icon;
        typeIcon.color = employee.type.color;

        employee.onLevelUp.AddListener(UpdateLevel);
    }

    private void UpdateLevel()
    {
        levelLabel.text = $"LV{employee.Level}";
        levelLabel.color = levelColors[employee.Level - 1];
    }
}
