using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance. Do not access on Awake.
    /// </summary>
    public static GameManager main;

    private int _money = 10000;
    private int _quarter = 1;

    //do not modify directly
    public List<Employee> employees = new();

    //caclulated & cached
    /// <summary>
    /// Calculated quarterly salary estimate. Is not guaranteed to equal actuall money consumption.
    /// </summary>
    public int quarterlySalary;
    public int quarterlyRevenue;

    public int Money => _money;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        _quarter = 1;
    }

    public void UpdateQuarter()
    {
        foreach (Employee e in employees)
        {

            e.UpdateQuarter();
        }
    }

    public void AddMoney(int amount)
    {
        _money += amount;
    }

    public void RecruitEmployee(Employee employee)
    {
        employees.Add(employee);
        RecalculateSalary();
    }

    public void RemoveMoney(int amount)
    {
        _money -= amount;
    }

    public void FireEmployee(Employee employee)
    {
        //note: employee must be removed from project before firing!
        employees.Remove(employee);
        RecalculateSalary();
    }

    public void RecalculateSalary()
    {
        int c = 0;
        foreach (Employee e in employees)
        {
            c += e.GetSalary();
        }
        quarterlySalary = c;
    }
}
