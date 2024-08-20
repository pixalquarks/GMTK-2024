using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ShowInInspector]
public struct EmployeeSkillset
{
    public static EmployeeSkillset Zero = new EmployeeSkillset() { ability = 0, passion = 0, speed = 0, cooperation = 0, potential = 0 };

    public float ability;
    public float passion;
    public float speed;
    public float cooperation;
    public float potential;

    public float GetValue(int id)
    {
        switch (id)
        {
            case 0:
                return ability;
            case 1:
                return passion;
            case 2:
                return speed;
            case 3:
                return cooperation;
            default:
                return potential;
        }
    }
}