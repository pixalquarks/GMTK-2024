using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Employee : MonoBehaviour
{
    #region constants
    public const int MAX_LEVEL = 3;
    public const int SP_PER_LEVEL = 3;
    #endregion

    public Rigidbody2D rigid;
    public EmployeeRenderer srenderer;

    #region stats
    //Initial stats
    //should not be changed during gameplay, only by EmployeeGenerator
    public string displayName = "";
    public EmployeeRole role = EmployeeRole.Programmer;
    public EmployeeType type;
    [System.NonSerialized] [ShowInInspector, ReadOnly] public EmployeeSkillset baseSkillset;
    public int baseSalary = 600;
    public EmployeeTrait mainTrait, subTrait;
    #endregion

    #region vars
    //changes during gameplay
    [ShowInInspector, ReadOnly] private int level = 1;
    private int skillPoints = 0;
    private int exp = 0;

    /// <summary>
    /// Cached skill set increase. Affected by Genre compatibility, Potential and Cooperation stats, and Traits. Should be summed with baseSkillSet.
    /// </summary>
    [System.NonSerialized] public EmployeeSkillset calculatedSkillBonus = default;
    [System.NonSerialized] public Project project;

    private bool _isFirstQuarter = false;
    private bool _employed = false;
    public bool killed = false;

    public enum EmployeeRole
    {
        Programmer,
        Artist
    }
    #endregion

    #region properties
    public int Level => level;
    public int SkillPoints => skillPoints;
    public int Exp => exp;
    public bool Employed => _employed;

    //skillsets
    public float SkillAbility => baseSkillset.ability + calculatedSkillBonus.ability;
    public float SkillPassion => baseSkillset.passion + calculatedSkillBonus.passion;
    public float SkillSpeed => baseSkillset.speed + calculatedSkillBonus.speed;
    public float SkillCooperation => baseSkillset.cooperation + calculatedSkillBonus.cooperation;
    public float SkillPotential => baseSkillset.potential + calculatedSkillBonus.potential;
    #endregion

    #region events
    public LevelUpEvent onLevelUp = new();

    [System.Serializable] public class LevelUpEvent : UnityEvent { }
    #endregion

    private void Start()
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        killed = false;
    }

    public int GetSalary()
    {
        int multiplier = level == 3 ? 5 : level == 2 ? 2 : 1;
        return multiplier * baseSalary;
    }

    public int GetRequiredExp()
    {
        return 100 + (200 <<  level);
    }

    public bool CanBePicked() {
        return !killed;
    }

    public void OnRecruited()
    {
        GameManager.main.RemoveMoney(baseSalary);
        _isFirstQuarter = true;//already paid for this quarter
        _employed = true;
    }

    public void OnFired()
    {
        _employed = false;
        killed = true;
        StartCoroutine(IKill());
    }

    IEnumerator IKill() {
        float t = 0;
        var start = transform.localScale;
        while (t < 1f) {
            t += Time.deltaTime * 2f;
            transform.localScale = start * (1 - t);

            yield return null;
        }
        Destroy(gameObject);
    }

    public void OnProjectJoined()
    {
        CalculateSkillBonus();
    }

    public void OnProjectLeft()
    {
        CalculateSkillBonus();
    }

    public void UpdateQuarter()
    {
        if(!_isFirstQuarter) GameManager.main.RemoveMoney(GetSalary());
        else _isFirstQuarter = false;
        //todo trigger trait

        if(project is not null)
        {
            if(project.Status == Project.ProjectStatus.Development)
            {
                AddExp(Mathf.RoundToInt(70f * (1f + (project.requiredLoad - 3f) / 3f)));
            }
            else if(project.Status == Project.ProjectStatus.Release)
            {
                AddExp(Mathf.RoundToInt(40f * (1f + (project.requiredLoad - 3f) / 3f)));
            }
        }
    }

    public void AddExp(int amount)
    {
        if (level >= MAX_LEVEL) return;
        exp += amount;
        if (exp >= GetRequiredExp()) LevelUp();
    }

    public void SetLevel(int l)
    {
        level = l;
    }

    public void SpendSkillPoint(int id)
    {
        switch (id)
        {
            case 0:
                baseSkillset.ability += 1f;
                break;
            case 1:
                baseSkillset.passion += 1f;
                break;
            case 2:
                baseSkillset.speed += 1f;
                break;
            case 3:
                baseSkillset.cooperation += 1f;
                break;
            case 4:
                baseSkillset.potential += 1f;
                break;
        }
        GameManager.main.RecalculateSalary();
    }

    private void LevelUp()
    {
        exp = 0;
        level++;
        skillPoints += SP_PER_LEVEL;
        GameManager.main.RecalculateSalary();
        CalculateSkillBonus();

        onLevelUp.Invoke();
    }

    private void CalculateSkillBonus()
    {
        calculatedSkillBonus = EmployeeSkillset.Zero;
        float a = 0, p = 0, s = 0, bonus = 0;

        if(project is not null)
        {
            //calculate genre bonus
            float g = type.GetGenreBonus(project.genre);
            bonus += g;
            a += g;

            //calculate Coop
            bonus += (project.employees.Count - 1) * 0.1f * SkillCooperation;
        }

        //calculate Potential
        bonus += (level - 1) * 0.2f * SkillPotential;

        //todo calculate trait

        p += bonus;
        s += bonus;
        calculatedSkillBonus.ability = a;
        calculatedSkillBonus.passion = p;
        calculatedSkillBonus.speed = s;

        if (project is not null)
        {
            project.RecalculateLoad();
        }
    }

    public float GetLoad()
    {
        return 1 + SkillAbility * 0.1f;
    }
}
