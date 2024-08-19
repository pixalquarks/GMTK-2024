using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeGenerator : MonoBehaviour
{
    #region stats
    [Header("Generation Settings")]
    public float salaryMultiplier = 1f;

    public float skillPointSalaryIncrease = 130; //internal value of skill points
    public float minSkillPoints = 4f;
    public float maxSkillPoints = 6.5f;

    public float artistChance = 0.3f;

    public int baseSalary = 400;
    public int levelMultiplier = 3;
    [Tooltip("The first element corresponds to the chance of a level 2 employee spawning, the second element corresponds to level 3 and so on.")]
    public float[] levelChances = { 0.20f, 0.05f };

    public int traitSalaryIncrease = 100;
    public float mainTraitChance = 0.6f;
    public float subTraitChance = 0.1f;
    #endregion

    [Header("Data")]
    public EmployeeType[] employeeTypes = { };
    public EmployeeTrait[] employeeTraits = { };

    [Header("References")]
    [SerializeField] private Employee prefab;
    [SerializeField] private Transform employeeRoot;
    [SerializeField] private NameGenerator nameGenerator;

    private float[] spMark = new float[5];

    public void InitializeEmployee(Employee employee)
    {
        employee.displayName = nameGenerator.Generate();
        employee.name = $"Employee_{employee.displayName}";

        float sp = Random.Range(minSkillPoints, maxSkillPoints);
        int l = 1;

        for (int i = 0; i < levelChances.Length; i++)
        {
            if (Random.Range(0f, 1f) < levelChances[i]) l = i + 2;
        }
        sp += (l - 1) * Employee.SP_PER_LEVEL;

        //set base salary
        employee.SetLevel(l);
        employee.baseSalary = Mathf.CeilToInt((baseSalary * Mathf.Pow(levelMultiplier, l - 1) + sp * skillPointSalaryIncrease) * Random.Range(0.75f, 1.1f) * salaryMultiplier);

        //set type and role
        employee.role = Random.Range(0f, 1f) > artistChance ? Employee.EmployeeRole.Programmer : Employee.EmployeeRole.Artist;
        employee.type = employeeTypes[Random.Range(0, employeeTypes.Length)];

        //set trait
        if (employeeTraits.Length > 0 && Random.Range(0f, 1f) < mainTraitChance)
        {
            employee.mainTrait = employeeTraits[Random.Range(0, employeeTraits.Length)];
            baseSalary += traitSalaryIncrease;
            if(Random.Range(0f, 1f) < subTraitChance)
            {
                var sub = employeeTraits[Random.Range(0, employeeTraits.Length)];
                if (!sub.mainTraitOnly)
                {
                    employee.subTrait = sub;
                    baseSalary += traitSalaryIncrease;
                }
            }
        }

        //divvy up sp
        for(int i = 0; i < 5; i++)
        {
            spMark[i] = Random.Range(0f, sp);
        }
        System.Array.Sort(spMark);

        for (int i = 4; i > 0 ; i--)
        {
            spMark[i] -= spMark[i - 1];
        }
        EmployeeSkillset skillset = default;
        skillset.ability = spMark[0];
        skillset.potential = spMark[1];
        skillset.cooperation = spMark[2];
        skillset.passion = spMark[3];
        skillset.speed = spMark[4];

        employee.baseSkillset = skillset;
    }

    public Employee Generate(Vector3 pos)
    {
        Employee e = Instantiate(prefab, pos, Quaternion.identity, employeeRoot);
        InitializeEmployee(e);

        return e;
    }
}
