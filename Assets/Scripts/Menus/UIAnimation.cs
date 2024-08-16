using System;
using System.Collections;
using System.Collections.Generic;
using CozyChaosSpring2024;
using DG.Tweening;
using UnityEngine;

namespace CozyChaosSpring2024
{
    public class UIAnimation : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private Vector2 _defaultSizeDelta;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _defaultSizeDelta = _rectTransform.sizeDelta;
        }

        public void Shrink() => _rectTransform.DOSizeDelta(new Vector2(0, 0), MainMenuManager.UIElemAnimDuration).SetEase(Ease.InCubic);

        public void Grow() => _rectTransform.DOSizeDelta(_defaultSizeDelta, MainMenuManager.UIElemAnimDuration).SetEase(Ease.OutCubic);
    }
}
