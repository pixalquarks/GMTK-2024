using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectGenerator : MonoBehaviour
{
    #region stats
    [Header("Generation Settings")]
    public float baseRevenue = 1000;
    public float revenuePerEmployee = 700;
    public float revenuePerExtraLoad = 1800;
    public float extraLoadMin = -0.9f;
    public float extraLoadMax = 1f;

    #endregion

    [Header("Data")]
    public Genre[] genres = { };

    [Header("References")]
    [SerializeField] private Project prefab;
    [SerializeField] private Transform projectRoot;
}