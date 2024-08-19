using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCursor : MonoBehaviour
{
    private const int PROJECT_LAYER = 6;
    private const int EMPLOYEE_LAYER = 7;
    private const int SLOTS_LAYER = 6;

    [SerializeField] private Transform entityRoot;
    [SerializeField] private SpriteRenderer highlight;
    [SerializeField] private float cursorRadius = 10f;

    private Camera cam;
    private CursorState state;

    private Employee employee;
    private Project project;

    private Project lastHoverProject;
    private bool hoverOverFire = false;

    private Collider2D[] tmp = new Collider2D[15];

    public enum CursorState
    {
        Idle,
        PickupEmployee,
        PickupProject
    }


    private void Awake()
    {
        cam = Camera.main;
        SetState(CursorState.Idle);
    }

    private void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        transform.position = mousePos;

        if(state == CursorState.PickupProject)
        {
            //todo
        }
        else if(state == CursorState.PickupEmployee)
        {
            if (!Input.GetMouseButton(0))
            {
                DropEmployee();
                return;
            }

            //find project
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                //raycast to find employee
                int n = Physics2D.OverlapCircleNonAlloc(mousePos, 1f, tmp, 1 << EMPLOYEE_LAYER);
                if(n > 0)
                {
                    //sort by distance
                    var c = GetClosest(mousePos, n);
                    PickupEmployee(c.GetComponent<Employee>());
                    return;
                }
                else
                {
                    //raycast to find project
                    //todo
                }
            }
        }
    }

    private void SetState(CursorState state)
    {
        this.state = state;

        switch (state)
        {
            case CursorState.Idle:
                highlight.enabled = false;
                break;
            case CursorState.PickupEmployee:
                highlight.enabled = false;
                lastHoverProject = null;
                break;
        }
    }

    private Collider2D GetClosest(Vector3 pos, int n)
    {
        Collider2D closest = tmp[0];
        float tx = pos.x - closest.transform.position.x;
        float ty = pos.y - closest.transform.position.y;
        float dst2 = tx * tx + ty * ty;

        for (int i = 1; i < n; i++)
        {
            tx = pos.x - tmp[n].transform.position.x;
            ty = pos.y - tmp[n].transform.position.y;
            float cur = tx * tx + ty * ty;

            if(cur < dst2)
            {
                dst2 = cur;
                closest = tmp[n];
            }
        }

        return closest;
    }

    private void PickupEmployee(Employee e)
    {
        e.rigid.bodyType = RigidbodyType2D.Static;
        e.transform.SetParent(transform, false);
        e.transform.localPosition = Vector3.zero;
        employee = e;

        if (!employee.Employed)
        {
            GameManager.main.RecruitEmployee(employee);
        }
        SetState(CursorState.PickupEmployee);
    }

    private void DropEmployee()
    {
        employee.rigid.bodyType = RigidbodyType2D.Dynamic;
        employee.transform.SetParent(entityRoot);

        if (hoverOverFire)
        {
            //todo fire employee
            GameManager.main.FireEmployee(employee);
        }
        else
        {
            employee.rigid.bodyType = RigidbodyType2D.Dynamic;
        }

        employee = null;
        SetState(CursorState.Idle);
    }
}
