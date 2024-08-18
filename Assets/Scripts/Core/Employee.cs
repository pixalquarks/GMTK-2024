using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : MonoBehaviour
{
    #region stats
    //Initial stats
    //should not be changed during gameplay, only by EmployeeGenerator
    [Header("Initial Stats")]
    public string displayName = "";
    public EmployeeRole role = EmployeeRole.Programmer;
    public EmployeeSkillset baseSkillset;
    public int baseSalary = 600;
    public EmployeeTrait mainTrait, subTrait;

    //changes during gameplay
    private int _level = 1;
    private int _skillPoints = 0;
    /// <summary>
    /// Cached skill set increase. Affected by Genre compatibility, Potential and Cooperation stats only. Should be summed with baseSkillSet.
    /// </summary>
    private EmployeeSkillset _calculatedSkillBonus = default;
    private int _exp = 0;

    public enum EmployeeRole
    {
        Programmer,
        Artist
    }
    #endregion

    #region properties
    public int Level => _level;
    public int SkillPoints => _skillPoints;
    public int Exp => _exp;

    //skillsets
    public float SkillAbility => baseSkillset.ability + _calculatedSkillBonus.ability;
    public float SkillPassion => baseSkillset.passion + _calculatedSkillBonus.passion;
    public float SkillSpeed => baseSkillset.speed + _calculatedSkillBonus.speed;
    public float SkillCooperation => baseSkillset.cooperation + _calculatedSkillBonus.cooperation;
    public float SkillPotential => baseSkillset.potential + _calculatedSkillBonus.potential;
    #endregion

    public int GetSalary()
    {
        int multiplier = _level == 3 ? 5 : _level == 2 ? 2 : 1;
        return multiplier * baseSalary;
    }
}
