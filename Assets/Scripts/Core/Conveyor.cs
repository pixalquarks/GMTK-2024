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
    [SerializeField] private Transform beltParent;
    [SerializeField] private int belts = 9;
    [SerializeField] private Vector2 beltSize = new Vector2(3.5f, 4f);

    private float time;
    private float nextSpawnTime;
    private float beltProgression;

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
        beltProgression += delta * speed / (endPoint.position.x - startPoint.position.x);
        beltProgression %= 1f;
        UpdateBelts();
        if (time > nextSpawnTime) {
            nextSpawnTime = PollNextSpawnTime();
            time = 0;
            Spawn();
        }
    }

    private void UpdateBelts()
    {
        float dst = endPoint.position.x - startPoint.position.x;
        float d = 1f / belts;
        Debug.Log("dst "+ dst);
        for (int i = 0; i < belts; i++)
        {
            float offset = (d * i + beltProgression) % 1f;
            Debug.Log($"{i} : {offset}");
            Transform t = beltParent.GetChild(i);
            t.transform.position = startPoint.position + Vector3.right * offset * dst;
            if(offset < d * 0.5f)
            {
                t.localScale = new Vector3(offset / d * 2f * beltSize.x, beltSize.y, 1f);
            }
            else if(offset > 1f - d * 0.5f)
            {
                t.localScale = new Vector3((1f - offset) / d * 2f * beltSize.x, beltSize.y, 1f);
            }
            else
            {
                t.localScale = beltSize;
            }
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