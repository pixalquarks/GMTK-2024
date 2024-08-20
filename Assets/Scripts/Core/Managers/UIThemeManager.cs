using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIThemeManager : MonoBehaviour
{
    public static UIThemeManager main;

    public string[] projectStatusText = { };
    public Color[] projectStatusColors = { };
    public Color[] loadColors = { };
    public Color[] levelColors = { };

    public Sprite[] projectLevelBackgrounds = { };
    public Color[] projectLevelTitleColors = { };

    public Sprite programmerIcon, artistIcon;

    private void Awake()
    {
        main = this;
    }

    public Sprite GetRoleIcon(Employee.EmployeeRole role)
    {
        return role == Employee.EmployeeRole.Programmer ? programmerIcon : artistIcon;
    }

    public Color GetLoadColor(int i)
    {
        return loadColors[i % loadColors.Length];
    }

    public Color GetStatusColor(Project.ProjectStatus status)
    {
        return projectStatusColors[(int)status];
    }

    public string GetStatusText(Project.ProjectStatus status)
    {
        return projectStatusText[(int)status];
    }
}
