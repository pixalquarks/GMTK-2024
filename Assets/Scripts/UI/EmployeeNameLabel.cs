using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeNameLabel : MonoBehaviour
{
    [SerializeField] private Employee employee;
    [SerializeField] private TMP_Text label;

    private void Start()
    {
        label.text = employee.displayName;
        label.color = employee.type.color;
    }
}
