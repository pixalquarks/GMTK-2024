using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(BoxCollider2D))]
public class RectTransformCollider : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private BoxCollider2D col;

    private Vector2 lastSize = new Vector2(-1, -1);

    private void Update()
    {
        if (rect.sizeDelta != lastSize)
        {
            col.size = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
            lastSize = rect.sizeDelta;
        }
    }

    private void Reset()
    {
        rect = GetComponent<RectTransform>();
        col = GetComponent<BoxCollider2D>();
    }

    //todo add listener
}
