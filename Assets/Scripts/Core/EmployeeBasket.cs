using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeBasket : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI salaryLabel;
    [SerializeField] private Transform employeeRoot;

    private Employee employee;
    private Conveyor conveyor;

    public void Set(Employee employee, Conveyor conv) {
        this.employee = employee;
        conveyor = conv;
        employee.transform.SetParent(this.employeeRoot);
        employee.transform.localPosition = Vector3.zero;

        salaryLabel.text = $"${employee.GetSalary():N0}";

        GameManager.main.onPlayerRecruit.AddListener(OnEmployeeHire);
    }

    private void Update() {
        if (!GameManager.main.IsPlaying) return;

        transform.position += conveyor.speed * Vector3.right * GameManager.main.GameSpeed * Time.deltaTime;
        if(transform.position.x > conveyor.endPoint.position.x) {
            Destroy(gameObject);
        }
    }


    private void OnDestroy() {
        GameManager.main.onPlayerRecruit.RemoveListener(OnEmployeeHire);
    }

    private void OnEmployeeHire(Employee e) {
        if(e == employee) {
            //destroy this
            Destroy(gameObject);
        }
    }
}