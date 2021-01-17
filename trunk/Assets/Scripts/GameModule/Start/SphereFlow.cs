using Creator.Expand;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creator
{
    public class SphereFlow : MonoBehaviour
    {
        public Material mat;
        public float duration = 5;
        private float t = 0;
        private TweenerCore<float, float, FloatOptions> Handler;
        private List<Vector3> points = new List<Vector3>();
        // Start is called before the first frame update
        void Awake()
        {
            points.Clear();
            CreateNewTween();
        }

        private void Update()
        {
            t += Time.unscaledDeltaTime;
            if(t <= duration)
            {
                Vector3 pos = MathfUtil.BezierCurve(points, t);
                var forward = (pos - transform.position).normalized;
                if (forward != Vector3.zero)
                    transform.forward = forward;
                transform.position = pos;
                points.Add(transform.position);
            }
            else
            {
                t = 0;
                points.Clear();
                CreateNewTween();
            }

        }

        void CreateNewTween()
        {
            var num = Random.Range(1, 10);
            var list = new List<Vector3>(10);
            for (int i = 0; i < num; i++)
            {
                list.Add(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
            }
        }

        private void OnGUI()
        {
            if (mat == null) return;
            mat.SetPass(0);
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            for (int i = 0; i < points.Count - 1; ++i)
            {
                GL.Vertex(points[i]);
                GL.Vertex(points[i + 1]);
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}

