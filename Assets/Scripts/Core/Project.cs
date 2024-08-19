using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Project : MonoBehaviour
{
    #region constants
    private const float BASE_DEGREDATION = 50f;
    private const float PLAN_LIFETIME = 30f;
    private const float STRONG_UNDERSTAFF = 0.75f;
    private const float DEGRADE_SPEED = 2f;
    private const float DEGRADE_HEAL_SPEED = 3f;
    #endregion

    public ProjectRenderer prenderer;
    public Rigidbody2D rigid;

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
    private ProjectStatus status = ProjectStatus.None;
    private ProjectStatus nextStatus = ProjectStatus.Planned;
    public readonly List<Employee> employees = new();
    public int programmerCount = 0;
    public int artistCount = 0;

    public float currentLoad; //cached
    [ShowInInspector] private float degredation; //used also as a countdown in Planned projects
    [ShowInInspector] private float progress; //0~100

    public float currentSpeedBonus = 1f; //cached
    public float currentRevenueBonus = 1f; //cached

    public bool killed = false;

    public enum ProjectStatus
    {
        None, //for FSM purposes
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
    public float DegredationFraction => degredation / Mathf.Max(BASE_DEGREDATION, progress / requiredProgress * 100f);
    public float LoadFraction => currentLoad / requiredLoad;
    public float PlanExpireFraction => degredation / PLAN_LIFETIME;
    #endregion

    #region events
    public StatusUpdateEvent onStatusChanged = new();
    public EmployeeUpdateEvent onEmployeeChanged = new();

    [System.Serializable]
    public class StatusUpdateEvent : UnityEvent { }

    [System.Serializable]
    public class EmployeeUpdateEvent : UnityEvent { }
    #endregion

    public void UpdateQuarter()
    {
        if(status == ProjectStatus.Release)
        {
            GameManager.main.AddMoney(GetRevenue());
        }
    }

    public int GetRevenue()
    {
        return Mathf.RoundToInt(baseRevenue * currentRevenueBonus * Efficiency());
    }

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
        if (LoadFraction >= 1) return 1;
        return 1f - Mathf.Clamp01((1 - LoadFraction) / (1 - STRONG_UNDERSTAFF)) * 0.5f;
    }

    /// <summary>
    /// returns true on strong understaff.
    /// Strongly understaff projects start to degrade and does not progress.
    /// </summary>
    public bool IsUnderstaffed()
    {
        return LoadFraction < STRONG_UNDERSTAFF || programmerCount < minProgrammerCount || artistCount < minArtistCount;
    }

    public bool ShouldBeScrapped()
    {
        return degredation >= Mathf.Max(BASE_DEGREDATION, progress / requiredProgress * 100f) && IsUnderstaffed();
    }

    public void AddEmployee(Employee employee)
    {
        employees.Add(employee);
        employee.project = this;

        if (employee.role == Employee.EmployeeRole.Programmer) programmerCount++;
        else artistCount++;
        RecalculateLoad();
        employee.OnProjectJoined();
        onEmployeeChanged.Invoke();
    }

    public void RemoveEmployee(Employee employee)
    {
        employee.OnProjectLeft();
        employees.Remove(employee);
        employee.project = null;

        if (employee.role == Employee.EmployeeRole.Programmer) programmerCount--;
        else artistCount--;
        RecalculateLoad();
        onEmployeeChanged.Invoke();
    }

    public void RecalculateLoad()
    {
        currentLoad = 0f;
        currentSpeedBonus = 1f;
        currentRevenueBonus = 1f;
        foreach (Employee e in employees)
        {
            currentLoad += e.GetLoad();
            currentSpeedBonus += e.SkillSpeed * 0.05f;
            currentRevenueBonus += e.SkillPassion * 0.05f;
        }

        GameManager.main.RecalculateRevenue();
    }

    private void Kill()
    {
        GameManager.main.RemoveProject(this);
        killed = true;
        StartCoroutine(IKill());
    }

    IEnumerator IKill()
    {
        float t = 0;
        var start = transform.localScale;
        while(t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.localScale = start * (1 - t);

            yield return null;
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!GameManager.main.IsPlaying) return;
        float delta = Time.deltaTime * GameManager.main.GameSpeed;

        //auto state change
        if(nextStatus == ProjectStatus.None)
        {
            switch (status)
            {
                case ProjectStatus.Planned:
                    if (employees.Count > 0)
                    {
                        Debug.Log("Accept project!");
                        nextStatus = ProjectStatus.Development;
                    }
                    else if (degredation >= PLAN_LIFETIME) nextStatus = ProjectStatus.Scrapped;
                    break;
                case ProjectStatus.Development:
                    if (ShouldBeScrapped()) nextStatus = ProjectStatus.Scrapped;
                    else if (progress >= requiredProgress) nextStatus = ProjectStatus.Release;
                    break;
                case ProjectStatus.Release:
                    if (ShouldBeScrapped()) nextStatus = ProjectStatus.Scrapped;
                    break;
            }
        }

        //state init
        if (nextStatus != ProjectStatus.None)
        {
            status = nextStatus;
            nextStatus = ProjectStatus.None;
            RecalculateLoad();
            switch (status)
            {
                case ProjectStatus.Planned:
                    degredation = 0;
                    break;
                case ProjectStatus.Development:
                    GameManager.main.RemoveMoney(initialCost);
                    degredation = 0;
                    break;
                case ProjectStatus.Release:
                    progress = requiredProgress;
                    //todo stat change on release
                    break;
                case ProjectStatus.Scrapped:
                    break;
            }
            onStatusChanged.Invoke();
            GameManager.main.OnProjectStatusChange(this);
        }

        //state update
        switch (status)
        {
            case ProjectStatus.Planned:
                degredation += delta;
                break;
            case ProjectStatus.Development:
            case ProjectStatus.Release:
                if (IsUnderstaffed())
                {
                    degredation += delta * DEGRADE_SPEED;
                }
                else
                {
                    if(status == ProjectStatus.Development) progress += delta * Efficiency() * currentSpeedBonus;
                    if (degredation > 0)
                    {
                        degredation -= delta * DEGRADE_HEAL_SPEED;
                        if (degredation < 0) degredation = 0;
                    }
                }
                break;
            case ProjectStatus.Scrapped:
                if (employees.Count <= 0)
                {
                    Kill();
                }
                break;
        }
    }
}
