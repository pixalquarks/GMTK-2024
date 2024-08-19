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
    [SerializeField] private Image genreIcon, statusBackground, degradationBar;
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

    private List<ProjectEmployeeSlot> slots = new();
    private List<Image> loadBars = new();

    private void Start()
    {
        nameLabel.text = project.displayName;
        degradationBar.enabled = false;

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

        UpdateDegredation();
    }

    public void Rebuild()
    {
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
                break;
            case Project.ProjectStatus.Development:
                var c = progressBar.color;
                c.a = 1f;
                progressBar.color = c;
                UpdateProgress();
                break;
            case Project.ProjectStatus.Release:
                var c1 = progressBar.color;
                c1.a = 1f;
                progressBar.color = c1;
                SetBar(progressBar, 1);
                progressLabel.text = "Progress: 100%";
                break;
            case Project.ProjectStatus.Scrapped:
                var c2 = progressBar.color;
                c2.a = 0.5f;
                progressBar.color = c2;
                progressLabel.text = "Progress: --";
                break;
        }

        UpdateLoad();
        UpdateDegredation();

        artistCountLabel.text = $"<color={(project.artistCount >= project.minArtistCount ? "white" : "red")}>{project.artistCount}</color>/{project.minArtistCount}";
        programmerCountLabel.text = $"<color={(project.programmerCount >= project.minProgrammerCount ? "white" : "red")}>{project.programmerCount}</color>/{project.minProgrammerCount}";

        float b = (project.currentRevenueBonus * project.Efficiency() - 1f) * 100;
        string bst = "";
        if (project.Status == Project.ProjectStatus.Development || project.Status == Project.ProjectStatus.Release)
        {
            if (b < 0)
            {
                bst = $" <color=red>(-{-b:F1}%)</color>";
            }
            else
            {
                bst = $" <color=green>(+{b:F1}%)</color>";
            }
        }
        revenueLabel.text = $"${project.GetRevenue()}/Q{bst}";

        foreach (var s in slots)
        {
            s.Rebuild();
        }
    }

    private void UpdateProgress()
    {
        progressLabel.text = $"Progress: {project.ProgressFraction * 100f:F0} <color=green>(+{project.Efficiency() * project.currentSpeedBonus:F1}/s)</color>";
        SetBar(progressBar, project.ProgressFraction);
    }

    private void UpdateDegredation()
    {
        if(project.Status == Project.ProjectStatus.Scrapped)
        {
            degradationBar.enabled = true;
            SetBar(degradationBar, 1);
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
