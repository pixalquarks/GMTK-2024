using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectEmployeeSlot : MonoBehaviour
{
    [SerializeField] private Image indicator;
    public RectTransform rect;
    public Transform employeeParent;

    private Project project;
    private int id;

    public void Set(Project p, int i)
    {
        id = i;
        project = p;

        indicator.color = UIThemeManager.main.GetLoadColor(i);
        Rebuild();
    }

    public void Rebuild()
    {
        indicator.enabled = project.employees.Count > id;
    }
}
