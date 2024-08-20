using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance. Do not access on Awake.
    /// </summary>
    public static GameManager main;

    [Header("General Settings")]
    [SerializeField] private int money = 10000;
    [SerializeField] private int quarter = 1;

    public float secondsPerQuarter = 50f;
    public float difficultyPerQuarter = 0.25f;

    public ProjectGenerator projectGenerator;
    public EmployeeGenerator employeeGenerator;

    //project spawning
    [Header("Project Spawning")]
    [Tooltip("Per Quarter")]
    public int projectSpawnRate = 2;

    //game time. Different from global time.
    [ShowInInspector] private bool playing = true;
    [ShowInInspector] private bool cutscene = false;
    [ShowInInspector] private float timeScale = 1f;

    //do not modify directly
    public List<Project> projects = new();
    public List<Employee> employees = new();

    //caclulated & cached
    /// <summary>
    /// Calculated quarterly salary estimate. Is not guaranteed to equal actual money consumption.
    /// </summary>
    [System.NonSerialized] [ShowInInspector, ReadOnly] public int quarterlySalary;
    [System.NonSerialized] [ShowInInspector, ReadOnly] public int quarterlyRevenue;
    private float timePassed = 0;
    private float projectSpawnTimer = 0;

    public QuarterEndEvent onQuarterEnd = new();
    public QuarterEndEvent onNextQuarterStart = new();

    [System.Serializable] public class QuarterEndEvent : UnityEvent<int> { };

    public int Money => money;
    public int Quarter => quarter;
    public bool IsPlaying => playing && !cutscene;
    public float GameSpeed => IsPlaying ? timeScale : 0;
    public float QuarterFraction => timePassed / secondsPerQuarter;
    public float ProjectSpawnFraction => projectSpawnTimer / secondsPerQuarter;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        quarter = 1;
        timePassed = 0;
        projectGenerator.difficulty = 0;
        projectSpawnTimer = secondsPerQuarter / projectSpawnRate * 0.5f;
    }

    private void Update() {
        if (!IsPlaying) return;
        float delta = Time.deltaTime * GameSpeed;

        timePassed += delta;
        projectGenerator.difficulty += delta * difficultyPerQuarter * (1f / secondsPerQuarter);
        projectSpawnTimer += delta * projectSpawnRate;

        if(projectSpawnTimer > secondsPerQuarter) {
            projectSpawnTimer -= secondsPerQuarter;
            SpawnNewProject();
        }

        if (timePassed > secondsPerQuarter) {
            timePassed = secondsPerQuarter;
            UpdateQuarter();
        }
    }

    public void UpdateQuarter()
    {
        cutscene = true;
        projectSpawnTimer = secondsPerQuarter / projectSpawnRate * 0.5f;
        StartCoroutine(IUpdateQuarter());
    }

    IEnumerator IUpdateQuarter() {
        yield return null;
        foreach (Project p in projects) {
            p.UpdateQuarter();//todo move cam to project
        }

        foreach (Employee e in employees) {

            e.UpdateQuarter();
        }

        onQuarterEnd.Invoke(quarter);

        quarter++;
        timePassed = 0;

        if (quarter > 4) {
            //todo event
        }

        if (quarter > 7 && quarter % 4 == 1) {
            //todo revenue goal
        }

        onNextQuarterStart.Invoke(quarter);
        cutscene = false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
    }

    public void RecruitEmployee(Employee employee)
    {
        employees.Add(employee);
        employee.OnRecruited();
        RecalculateSalary();
    }

    public void FireEmployee(Employee employee)
    {
        //note: employee must be removed from project before firing!
        if(employee.project is not null) employee.project.RemoveEmployee(employee);
        employees.Remove(employee);
        employee.OnFired();
        RecalculateSalary();
    }

    public void SpawnNewProject() {
        //todo find distance and center
    }

    public void AddProject(Project project)
    {
        projects.Add(project);
    }

    public void RemoveProject(Project project)
    {
        //assume employees are already removed
        if (project.employees.Count > 0)
        {
            Debug.LogError("Error: Tried to remove a project with employees!");
        }
        projects.Remove(project);
    }

    public void RecalculateSalary()
    {
        int c = 0;
        foreach (Employee e in employees)
        {
            c += e.GetSalary();
        }
        quarterlySalary = c;
    }

    public void RecalculateRevenue()
    {
        int c = 0;
        foreach (Project e in projects)
        {
            c += e.GetRevenue();
        }
        quarterlyRevenue = c;
    }

    public void OnProjectStatusChange(Project project)
    {
        //todo cutscene
    }

    public void StartCutscene()
    {
        cutscene = true;
    }

    public void EndCutscene()
    {
        cutscene = false;
    }

    public void Pause()
    {
        playing = false;
    }

    public void Resume()
    {
        playing = true;
    }

    public void SetSpeed(float speed)
    {
        timeScale = speed;
    }
}
