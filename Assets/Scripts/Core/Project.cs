using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    #region constants
    private const float BASE_DEGREDATION = 50f;
    private const float PLAN_LIFETIME = 30f;
    private const float STRONG_UNDERSTAFF = 0.75f;
    private const float DEGRADE_SPEED = 2f;
    private const float DEGRADE_HEAL_SPEED = 3f;
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
    public float LoadFraction => currentLoad / requiredLoad;
    public float PlanExpireFraction => degredation / PLAN_LIFETIME;
    #endregion

    public bool CanAddEmployee(Employee employee)
    {
        return employees.Count < maxEmployees;
    }

    /// <summary>
    /// Progress and Revenue multiplier. Is reduced down to 0.5 depending on the strength of (weak) understaffing. Is  0 on strong understaffing.
    /// </summary>
    /// <returns></returns>
    public float Efficiency()
    {
        if (LoadFraction < STRONG_UNDERSTAFF) return 0;
        return 1f - Mathf.Clamp01((1 - LoadFraction) / (1 - STRONG_UNDERSTAFF)) * 0.5f;
    }

    /// <summary>
    /// returns true on strong understaff.
    /// Strongly understaff projects start to degrade and does not progress.
    /// </summary>
    public bool IsUnderstaffed()
    {
        return LoadFraction < STRONG_UNDERSTAFF;
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
