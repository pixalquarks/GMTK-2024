using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectGenerationTest : MonoBehaviour {
    public ProjectGenerator projectGenerator;
    public float delay = 0.5f;
    public int spawnCount = 5;

    private float timer;
    private int n;

    void Update() {
        timer += Time.deltaTime;
        if (timer > delay) {
            timer = 0;
            n++;

            projectGenerator.Generate(new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f)));

            if (n >= spawnCount) enabled = false;
        }
    }

    [Button("Generate")]
    void Generate() {
        projectGenerator.Generate(new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f)));
    }
}
