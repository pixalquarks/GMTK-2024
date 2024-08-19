using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeGenerationTest : MonoBehaviour
{
    public EmployeeGenerator employeeGenerator;
    public float delay = 0.5f;
    public int spawnCount = 10;

    private float timer;
    private int n;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > delay)
        {
            timer = 0;
            n++;

            employeeGenerator.Generate(new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f)));

            if (n >= spawnCount) enabled = false;
        }
    }
}
