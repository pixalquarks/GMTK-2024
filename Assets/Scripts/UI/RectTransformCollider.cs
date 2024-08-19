using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(BoxCollider2D))]
public class RectTransformCollider : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private BoxCollider2D col;

    private Vector2 lastSize = new Vector2(-100, -100);

    private void Update()
    {
        if (rect.rect.size != lastSize)
        {
            col.size = new Vector2(rect.rect.width * rect.transform.lossyScale.x / col.transform.lossyScale.x, rect.rect.height * rect.transform.lossyScale.y / col.transform.lossyScale.y);
            lastSize = rect.rect.size;
        }
    }

    private void Reset()
    {
        rect = GetComponent<RectTransform>();
        col = GetComponent<BoxCollider2D>();
    }

    //todo add listener
}
