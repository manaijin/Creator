using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creator
{
    public class ScaleTween : TweenBase
    {
        [Tooltip("起始比例")]
        public Vector3 startScale = Vector3.one;
        [Tooltip("结束比例")]
        public Vector3 endScale = Vector3.one;

        public override Tweener CreateHandler(float duration, Ease curve)
        {
            if (Handler != null) return Handler;
            transform.localScale = startScale;
            Handler = transform.DOScale(endScale, duration);
            Handler.SetEase(curve);
            return Handler;
        }
    }
}