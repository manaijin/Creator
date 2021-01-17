using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Creator
{
    public class ColorTween : TweenBase
    {
        [Tooltip("起始颜色")]
        public Color startColor = Color.white;
        [Tooltip("结束颜色")]
        public Color endColor = Color.white;

        public override Tweener CreateHandler(float duration, Ease curve)
        {
            if (Handler != null) return Handler;
            var img = GetComponent<Image>();
            if (img == null) return null;
            img.color = startColor;
            Handler = img.DOColor(endColor, duration);
            return Handler;
        }
    }
}