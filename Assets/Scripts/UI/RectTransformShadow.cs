using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformShadow : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private SpriteRenderer shadow;

    private Vector2 lastSize = new Vector2(-1, -1);

    private void Update()
    {
        if (rect.sizeDelta != lastSize)
        {
            shadow.size = new Vector2(rect.rect.width * rect.transform.lossyScale.x / shadow.transform.lossyScale.x, rect.rect.height * rect.transform.lossyScale.y / shadow.transform.lossyScale.y);
            lastSize = rect.sizeDelta;
        }
    }

    //todo add listener
}
