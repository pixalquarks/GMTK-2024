using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    #region constants
    private const float BASE_DEGREDATION = 50f;
    private const float PLANNED_PROJECT_LIFETIME = 30f;
    #endregion

    #region stats
    //Initial stats
    //should not be changed during gameplay, only by ProjectGenerator
    public string displayName;
    public Genre genre;

    public int maxEmployees;
    public int minProgrammerCount;
    public int minArtistCount;

    public float requiredLoad;
    public float requiredProgress = 100f; //usually 100 * requiredLoad. Varies by genre
    public int initialCost = 300;
    public int baseRevenue = 3000;
    #endregion

    #region vars
    private ProjectStatus status = ProjectStatus.Planned;
    public readonly List<Employee> employees = new();

    private float currentLoad; //cached
    private float degredation; //used also as a countdown in Planned projects
    private float progress; //0~100

    public enum ProjectStatus
    {
        Planned,
        Development,
        Release,
        Scrapped
    }
    #endregion

    #region properties
    public ProjectStatus Status => status;
    public bool IsValid => status == ProjectStatus.Development || status == ProjectStatus.Release;

    //ui fractions
    public float ProgressFraction => status == ProjectStatus.Development ? progress / requiredProgress : status == ProjectStatus.Release ? 1f : 0f;
    public float DegredationFraction => degredation / Mathf.Max(BASE_DEGREDATION, progress);
    #endregion

    public bool CanAddEmployee(Employee employee)
    {
        return employees.Count < maxEmployees;
    }

    public void AddEmployee(Employee employee)
    {
        employees.Add(employee);
        employee.project = this;
        RecalculateLoad();
        employee.OnProjectJoined();
    }

    public void RemoveEmployee(Employee employee)
    {
        employee.OnProjectLeft();
        employees.Remove(employee);
        employee.project = null;
        RecalculateLoad();
    }

    public void RecalculateLoad()
    {
        currentLoad = 0f;
        foreach (Employee e in employees)
        {
            currentLoad += 1 + e.SkillAbility * 0.1f;
        }
    }
}
