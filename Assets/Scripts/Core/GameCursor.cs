using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCursor : MonoBehaviour
{
    private const int PROJECT_LAYER = 6;
    private const int EMPLOYEE_LAYER = 7;
    private const int SLOTS_LAYER = 8;
    private const int SLOTTED_EMPLOYEE_LAYER = 9;

    [SerializeField] private Transform entityRoot;
    [SerializeField] private SpriteRenderer highlight;
    [SerializeField] private float cursorRadius = 10f;

    private Camera cam;
    private CursorState state;

    private Employee employee;
    private Project project;

    private Project lastHoverProject;
    private bool hasHoverProject = false;
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
            if (!Input.GetMouseButton(0))
            {
                DropProject();
                return;
            }
        }
        else if(state == CursorState.PickupEmployee)
        {
            if (!Input.GetMouseButton(0))
            {
                DropEmployee();
                return;
            }

            if(hasHoverProject && (lastHoverProject == null || !lastHoverProject.CanAddEmployee(employee)))
            {
                hasHoverProject = false;
                lastHoverProject = null;
                highlight.enabled = false;
            }

            //find project slot
            Project proj = null;
            int n = Physics2D.OverlapCircleNonAlloc(mousePos, 1f, tmp, 1 << SLOTS_LAYER);
            if (n > 0)
            {
                //sort by distance
                var c = GetClosest(mousePos, n);
                proj = c.GetComponent<ProjectSlotsRedirect>().project;

                if (!proj.CanAddEmployee(employee)) proj = null;
            }

            if(proj != lastHoverProject)
            {
                if(proj is not null)
                {
                    hasHoverProject = true;
                    lastHoverProject = proj;
                    highlight.enabled = true;

                    highlight.transform.position = proj.prenderer.slotsRect.position;
                    highlight.size = proj.prenderer.slotsRect.rect.size * proj.prenderer.slotsRect.lossyScale;
                }
                else
                {
                    hasHoverProject = false;
                    lastHoverProject = null;
                    highlight.enabled = false;
                }
            }

            if (hasHoverProject)
            {
                highlight.transform.position = proj.prenderer.slotsRect.position;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Employee e = null;

                //attempt to grab an employee from slots
                int n = Physics2D.OverlapCircleNonAlloc(mousePos, 1f, tmp, 1 << SLOTS_LAYER);
                if (n > 0)
                {
                    var c = GetClosest(mousePos, n);
                    var proj = c.GetComponent<ProjectSlotsRedirect>().project;
                    e = proj.prenderer.AttemptCastEmployee(mousePos, 5f);
                }

                //fallback to casting world
                if(e is null)
                {
                    n = Physics2D.OverlapCircleNonAlloc(mousePos, 1f, tmp, 1 << EMPLOYEE_LAYER);
                    if (n > 0)
                    {
                        e = GetClosest(mousePos, n).GetComponent<Employee>();
                    }
                }

                if(e is not null)
                {
                    PickupEmployee(e);
                    return;
                }
                else
                {
                    //pick up project
                    n = Physics2D.OverlapCircleNonAlloc(mousePos, 1f, tmp, 1 << PROJECT_LAYER);
                    if (n > 0)
                    {
                        var c = GetClosest(mousePos, n);
                        var proj = c.GetComponent<ProjectSlotsRedirect>().project;
                        PickupProject(proj);
                        return;
                    }
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
                hasHoverProject = false;
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
            tx = pos.x - tmp[i].transform.position.x;
            ty = pos.y - tmp[i].transform.position.y;
            float cur = tx * tx + ty * ty;

            if(cur < dst2)
            {
                dst2 = cur;
                closest = tmp[i];
            }
        }

        return closest;
    }

    private void PickupEmployee(Employee e)
    {
        if (e.project != null)
        {
            e.project.RemoveEmployee(e);
        }

        e.rigid.simulated = false;
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
        employee.transform.SetParent(entityRoot);

        if (hasHoverProject && lastHoverProject.CanAddEmployee(employee))
        {
            employee.transform.SetParent(lastHoverProject.prenderer.GetSlotParent(lastHoverProject.employees.Count), false);
            employee.transform.localPosition = Vector3.zero;
            lastHoverProject.AddEmployee(employee);
            employee.rigid.simulated = false;
        }
        else if (hoverOverFire)
        {
            //todo fire employee
            GameManager.main.FireEmployee(employee);
        }
        else
        {
            employee.gameObject.layer = EMPLOYEE_LAYER;
            employee.rigid.simulated = true;
        }

        employee = null;
        SetState(CursorState.Idle);
    }

    private void PickupProject(Project p)
    {
        p.rigid.bodyType = RigidbodyType2D.Static;
        p.transform.SetParent(transform);
        project = p;

        SetState(CursorState.PickupProject);
    }

    private void DropProject()
    {
        project.transform.SetParent(entityRoot);
        project.rigid.bodyType = RigidbodyType2D.Dynamic;
        project = null;

        SetState(CursorState.Idle);
    }
}
