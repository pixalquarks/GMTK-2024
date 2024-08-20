using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectRenderer : MonoBehaviour
{
    [SerializeField] private Project project;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Image genreIcon, statusBackground, degradationBar, titleBackground, background;
    [SerializeField] private TextMeshProUGUI statusLabel;

    [Header("Employees")]
    [SerializeField] private ProjectEmployeeSlot slotPrefab;
    [SerializeField] private Transform slotRoot;
    public RectTransform slotsRect;

    [Header("Bars")]
    [SerializeField] private Image loadBarPrefab;
    [SerializeField] private Transform loadBarRoot;
    [SerializeField] private TextMeshProUGUI progressLabel, loadLabel, artistCountLabel, programmerCountLabel, revenueLabel;
    [SerializeField] private Image progressBar;

    [Header("Popups")]
    [SerializeField] private GameObject investmentPopup;
    [SerializeField] private GameObject understaffedPopup;
    [SerializeField] private TextMeshProUGUI investmentLabel;

    private List<ProjectEmployeeSlot> slots = new();
    private List<Image> loadBars = new();

    private void Start()
    {
        nameLabel.text = project.displayName;
        degradationBar.enabled = false;

        titleBackground.color = UIThemeManager.main.projectLevelTitleColors[project.level - 1];
        background.sprite = UIThemeManager.main.projectLevelBackgrounds[project.level - 1];

        slotRoot.ClearChildren();
        slots.Clear();
        for(int i = 0; i < project.maxEmployees; i++)
        {
            var s = Instantiate(slotPrefab, slotRoot);
            s.Set(project, i);
            slots.Add(s);
        }

        loadBars.Clear();
        for(int i = 0;i < project.maxEmployees; i++)
        {
            var t = Instantiate(loadBarPrefab, loadBarRoot);
            t.color = UIThemeManager.main.GetLoadColor(i);
            loadBars.Add(t);
        }

        project.onEmployeeChanged.AddListener(Rebuild);
        project.onStatusChanged.AddListener(Rebuild);

        Rebuild();
    }

    private void Update()
    {
        if(project.Status == Project.ProjectStatus.Development)
        {
            UpdateProgress();
        }
        else if(project.Status == Project.ProjectStatus.Planned)
           {
            investmentPopup.transform.position = slotRoot.transform.position;
        }

        UpdateDegredation();
    }

    public Transform GetSlotParent(int i)
    {
        return slots[i].employeeParent;
    }

    public void SortEmployees()
    {
        //pain
        for (int i = 0; i < project.employees.Count; i++)
        {
            project.employees[i].transform.SetParent(GetSlotParent(i), false);
            project.employees[i].transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Returns an employee in the given range
    /// </summary>
    public Employee AttemptCastEmployee(Vector3 pos, float distance)
    {
        float d2 = distance * distance;
        foreach (var e in project.employees)
        {
            if ((e.transform.position - pos).sqrMagnitude < d2) return e;
        }
        return null;
    }

    public void Rebuild()
    {
        understaffedPopup.SetActive(false);
        genreIcon.sprite = project.genre.icon;
        genreIcon.color = project.genre.color;

        statusBackground.color = UIThemeManager.main.GetStatusColor(project.Status);
        statusLabel.text = UIThemeManager.main.GetStatusText(project.Status);

        //todo initialcost

        switch (project.Status)
        {
            case Project.ProjectStatus.Planned:
                SetBar(progressBar, 0);
                progressLabel.text = "Progress: 0%";

                investmentPopup.SetActive(true);
                investmentPopup.transform.position = slotRoot.transform.position;
                investmentLabel.text = $"Invest ${project.initialCost}";
                break;
            case Project.ProjectStatus.Development:
                var c = progressBar.color;
                c.a = 1f;
                progressBar.color = c;
                UpdateProgress();
                investmentPopup.SetActive(false);
                break;
            case Project.ProjectStatus.Release:
                var c1 = progressBar.color;
                c1.a = 1f;
                progressBar.color = c1;
                SetBar(progressBar, 1);
                progressLabel.text = "Progress: 100%";
                investmentPopup.SetActive(false);
                break;
            case Project.ProjectStatus.Scrapped:
                var c2 = progressBar.color;
                c2.a = 0.5f;
                progressBar.color = c2;
                progressLabel.text = "Progress: --";
                investmentPopup.SetActive(false);
                break;
        }

        UpdateLoad();
        UpdateDegredation();

        artistCountLabel.text = $"<color={(project.artistCount >= project.minArtistCount ? "white" : "red")}>{project.artistCount}</color>/{project.minArtistCount}";
        programmerCountLabel.text = $"<color={(project.programmerCount >= project.minProgrammerCount ? "white" : "red")}>{project.programmerCount}</color>/{project.minProgrammerCount}";

        float b = (project.currentRevenueBonus * project.Efficiency() - 1f) * 100;
        if (project.Status == Project.ProjectStatus.Development || project.Status == Project.ProjectStatus.Release) {
            string bst = "";
            if (b < 0) {
                bst = $" <color=red>(-{-b:F1}%)</color>";
            }
            else {
                bst = $" <color=green>(+{b:F1}%)</color>";
            }
            revenueLabel.text = $"${project.GetRevenue():N0}/Q{bst}";
        }
        else if(project.Status == Project.ProjectStatus.Planned) {
            revenueLabel.text = $"${project.baseRevenue:N0}/Q";
        }
        else {
            revenueLabel.text = "Scrapped";
        }

        foreach (var s in slots)
        {
            s.Rebuild();
        }
    }

    private void UpdateProgress()
    {
        progressLabel.text = $"Progress: {project.ProgressFraction * 100f:F0}% <color=green>(+{project.Efficiency() * project.currentSpeedBonus:F1}/s)</color>";
        SetBar(progressBar, project.ProgressFraction);
    }

    private void UpdateDegredation()
    {
        if(project.Status == Project.ProjectStatus.Scrapped)
        {
            if(!degradationBar.enabled) degradationBar.enabled = true;
            SetBar(degradationBar, 1);
            if(understaffedPopup.activeSelf) understaffedPopup.SetActive(false);
            return;
        }

        float f = project.Status == Project.ProjectStatus.Planned ? project.PlanExpireFraction : project.DegredationFraction;

        if (f > 0.001f)
        {
            if (!degradationBar.enabled) degradationBar.enabled = true;
            SetBar(degradationBar, f);
        }
        else
        {
            if (degradationBar.enabled) degradationBar.enabled = false;
        }

        bool deg = project.IsUnderstaffed() && project.Status != Project.ProjectStatus.Planned;
        if (understaffedPopup.activeSelf != deg) understaffedPopup.SetActive(deg);
    }

    private void SetBar(Image i, float f)
    {
        i.rectTransform.anchorMin = Vector2.zero;
        i.rectTransform .anchorMax = new Vector2(f, 1);
    }

    private void UpdateLoad()
    {
        loadLabel.text = $"Load: <color={(project.LoadFraction >= 1f ? "white" : "red")}>{project.currentLoad:F2}</color>/{project.requiredLoad:F1}";

        float left = 0;
        for(int i = 0; i < project.maxEmployees; i++)
        {
            if(i >= project.employees.Count)
            {
                loadBars[i].enabled = false;
            }
            else
            {
                loadBars[i].enabled = true;
                float f = project.employees[i].GetLoad() / project.requiredLoad;
                loadBars[i].rectTransform.anchorMin = new Vector2(left, 0);
                loadBars[i].rectTransform.anchorMax = new Vector2(Mathf.Min(1, left + f), 1);
                left += f;
            }
        }
    }
}
