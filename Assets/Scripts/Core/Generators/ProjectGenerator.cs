using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Employee;

public class ProjectGenerator : MonoBehaviour
{
    #region stats
    [Header("Generation Settings")]
    [Tooltip("scales over time, approx. 1 per year")]
    public float difficulty = 0; //scales over time, approx. 1 per year

    public float baseRevenue = 1000;
    public float revenuePerEmployee = 700;
    public float revenuePerExtraLoad = 1800;
    public DifficultyScale extraLoadMin = default;
    public DifficultyScale extraLoadMax = default;

    public int baseTeamSize = 2;
    public float teamSizeToLoadOffset = -1f;
    public DifficultyScale teamSizeOffset = default;
    public DifficultyScale maxTeamSize = default;
    public float introduceArtistDifficulty = 1.5f;
    public float requireArtistDifficulty = 3f;
    public DifficultyScale requiredArtistPercentage = default;
    public DifficultyScale requiredProgrammerPercentage = default;

    public float baseRequiredProgress = 100f;
    public float progressPerEmployee = 20f;

    public DifficultyScale initialCostPercentage = default;

    [System.Serializable]
    public struct DifficultyScale {
        public float start;
        public float scale;
        public float clamp;
        public float offset;

        public float GetValue(float diff) {
            if(scale > 0) {
                return Mathf.Clamp(scale * (diff - offset) + start, start, clamp);
            }
            return Mathf.Clamp(scale * (diff - offset) + start, clamp, start);
        }
    }
    #endregion

    [Header("Data")]
    public Genre[] genres = { };

    [Header("References")]
    [SerializeField] private Project prefab;
    [SerializeField] private Transform projectRoot;

    private int GetLevel(int teamSize) {
        return Mathf.Clamp(teamSize / 2, 1, 5);
    }

    private string GetProjectName() {
        int length = Random.Range(2, 4);
        StringBuilder stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++) {
            char r = (char)Random.Range('A', 'Z' + 1);
            stringBuilder.Append(r);
        }

        return stringBuilder.ToString();
    }

    public void InitializeProject(Project project) {
        project.genre = genres[Random.Range(0, genres.Length)];
        project.displayName = $"{project.genre.name} {GetProjectName()}";
        project.name = $"Project_{project.displayName}";

        int n = Random.Range(baseTeamSize, Mathf.RoundToInt(maxTeamSize.GetValue(difficulty))) + Mathf.RoundToInt(teamSizeOffset.GetValue(difficulty));
        float revenue = (n - baseTeamSize) * revenuePerEmployee + baseRevenue;

        //decide required roles
        int artists = 0;
        if (difficulty > introduceArtistDifficulty) artists = Mathf.RoundToInt(Random.Range(0f, requiredArtistPercentage.GetValue(difficulty)) * n);
        int programmers = Mathf.RoundToInt(Random.Range(0f, requiredProgrammerPercentage.GetValue(difficulty)) * n);

        if (programmers < 1) programmers++;
        if (difficulty > requireArtistDifficulty && artists < 1) artists++;

        if(programmers + artists > n) {
            artists = n - programmers;
            if(artists == 0) {
                artists = 1;
                programmers = n - 1;
            }
        }

        project.maxEmployees = n;
        project.minArtistCount = artists;
        project.minProgrammerCount = programmers;
        project.level = GetLevel(n);

        //decide load
        float loadOffset = Random.Range(extraLoadMin.GetValue(difficulty), extraLoadMax.GetValue(difficulty)) * Random.Range(0.3f, 1f);
        float load = n + teamSizeToLoadOffset + loadOffset;
        if (load < 0.8f) load = Random.Range(0.8f, 1.5f);
        Debug.Log($"loadoffset: {loadOffset}");
        if (loadOffset < 0) loadOffset *= 0.2f;
        revenue = Mathf.Max(revenue * 0.8f, loadOffset * revenuePerExtraLoad + revenue);

        project.requiredLoad = load;
        project.requiredProgress = baseRequiredProgress + (n - baseTeamSize) * progressPerEmployee;

        project.baseRevenue = Mathf.RoundToInt(revenue / 10f) * 10;
        project.initialCost = Mathf.RoundToInt(revenue * Random.Range(0.1f, initialCostPercentage.GetValue(difficulty)));
    }

    public Project Generate(Vector3 pos) {
        Project e = Instantiate(prefab, pos, Quaternion.identity, projectRoot);
        InitializeProject(e);

        return e;
    }
}