using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{
    [SerializeField] private TextAsset json;
    private Names names;

    [System.Serializable]
    private struct Names
    {
        public string[] boys;
        public string[] girls;
    }

    private void Awake()
    {
        names = JsonUtility.FromJson<Names>(json.ToString());
    }

    public string Generate()
    {
        bool boy = Random.Range(0f, 1f) > 0.5f;
        if (boy)
        {
            return names.boys[Random.Range(0, names.boys.Length)] + " " + names.boys[Random.Range(0, names.boys.Length)];
        }
        else
        {
            return names.girls[Random.Range(0, names.girls.Length)] + " " + names.girls[Random.Range(0, names.girls.Length)];
        }
    }
}
