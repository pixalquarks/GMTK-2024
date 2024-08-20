using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour {
    private const int SLOTTED_EMPLOYEE_LAYER = 9;

    public float speed = 3.2f;
    public Transform startPoint, endPoint;

    public float spawnDelay = 7f;
    public float spawnDelayRandom = 2f;

    [SerializeField] private Transform basketParent;
    [SerializeField] private EmployeeBasket basketPrefab;
    [SerializeField] private EmployeeGenerator generator;

    private float time;
    private float nextSpawnTime;

    private void Start() {
        nextSpawnTime = PollNextSpawnTime();
    }

    private float PollNextSpawnTime() {
        return Random.Range(spawnDelayRandom-spawnDelayRandom, spawnDelayRandom) + spawnDelay;
    }

    private void Update() {
        if (!GameManager.main.IsPlaying) return;
        float delta = Time.deltaTime * GameManager.main.GameSpeed;

        time += delta;
        if(time > nextSpawnTime) {
            nextSpawnTime = PollNextSpawnTime();
            time = 0;
            Spawn();
        }
    }

    public void Spawn() {
        var e = generator.Generate(startPoint.position);
        e.gameObject.layer = SLOTTED_EMPLOYEE_LAYER;
        var b = Instantiate(basketPrefab, basketParent);
        b.transform.position = startPoint.position;
        b.Set(e, this);
    }
}