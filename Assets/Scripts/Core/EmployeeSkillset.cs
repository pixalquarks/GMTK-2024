using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EmployeeSkillset
{
    public static EmployeeSkillset Zero = new EmployeeSkillset() { ability = 0, passion = 0, speed = 0, cooperation = 0, potential = 0 };

    public float ability;
    public float passion;
    public float speed;
    public float cooperation;
    public float potential;
}