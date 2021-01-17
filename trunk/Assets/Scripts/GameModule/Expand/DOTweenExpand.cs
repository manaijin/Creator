using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Util;
using System;
using UnityEngine;

namespace Creator.Expand
{
    public static class DOTweenExpand
    {
        /// <summary>
        /// 根据当前位置做贝塞尔曲线运动
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="points"></param>
        /// <param name="duration"></param>
        public static TweenerCore<float, float, FloatOptions> DOBezierOffsetCurve(this Transform trans, Vector3[] offset, float duration, Action<float> cb = null)
        {
            var length = offset.Length;
            var points = new Vector3[length + 1];
            points[0] = trans.position;
            for (var i = 1; i < length; i++)
            {
                points[i] = trans.position + offset[i];
            }
            return DOBezierCurve(trans, points, duration, cb);
        }

        /// <summary>
        /// 对象做贝塞尔曲线运动
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="points"></param>
        /// <param name="duration"></param>
        public static TweenerCore<float, float, FloatOptions> DOBezierCurve(this Transform trans, Vector3[] points, float duration, Action<float> cb = null)
        {
            ulong n = (ulong)(points.Length - 1);
            float t = 0;
            var handler = DOTween.To(() => t, dt =>
            {
                t = dt;
                Vector3 pos = MathfUtil.BezierCurve(points, t);
                var forward = (pos - trans.position).normalized;
                if (forward != Vector3.zero)
                    trans.forward = forward;
                trans.position = pos;
                cb?.Invoke(dt);
            }, 1, duration);
            handler.SetEase(Ease.Linear);
            return handler;
        }
    }
}