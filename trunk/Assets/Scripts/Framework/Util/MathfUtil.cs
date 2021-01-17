using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Framework.Util
{
    public static class MathfUtil
    {
        /// <summary>
        /// n阶阶乘
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong Factorial(ulong n)
        {
            if (n <= 0) return 1;
            ulong result = 1;
            for (ulong i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        /// <summary>
        /// 有序排列组合数
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static ulong PermutationOrder(ulong n, ulong m)
        {
            if (n < m) return 0;
            return Factorial(n) / Factorial(n - m);
        }

        /// <summary>
        /// 不考虑顺序的排列组合
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static ulong PermutationDisorder(ulong n, ulong m)
        {
            return PermutationOrder(n, m) / Factorial(m);
        }

        /// <summary>
        /// 贝塞尔曲线计算
        /// </summary>
        /// <param name="points"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 BezierCurve(IEnumerable<Vector3> points, float t)
        {
            ulong n = (ulong)(points.Count() - 1);
            Vector3 pos = Vector3.zero;
            for (ulong i = 0; i <= n; i++)
            {
                pos += (MathfUtil.PermutationDisorder(n, i) * points.ElementAt((int)i)) * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            }
            return pos;
        }

        /// <summary>
        /// 贝塞尔曲线计算
        /// </summary>
        /// <param name="points"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float3 BezierCurve(IEnumerable<float3> points, float t)
        {
            ulong n = (ulong)(points.Count() - 1);
            float3 pos = float3.zero;
            for (ulong i = 0; i <= n; i++)
            {
                pos += (MathfUtil.PermutationDisorder(n, i) * points.ElementAt((int)i)) * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            }
            return pos;
        }

        /// <summary>
        /// 随机生成不同的随机数
        /// </summary>
        /// <param name="min">随机范围-最小值</param>
        /// <param name="max">随机范围-最大值</param>
        /// <param name="num">个数</param>
        /// <returns></returns>
        public static int[] RandomDifferent(int min, int max, int num = 1)
        {
            if (num <= 0)
            {
                Debug.Log($"num{num}小于0");
                return default;
            }
                
            if (max - min + 1 < num)
            {
                Debug.Log($"无法在{min}与{max}之间产生{num}个不同随机数");
                return default;
            }                

            var result = new int[num];
            List<int> pool = new List<int>();
            for(int i = 0; i <= max - min; i++)
            {
                pool.Add(i);
            }
            
            for(int i = 0; i < num; i++)
            {
                int count = pool.Count;
                int index = UnityEngine.Random.Range(0, count - 1);
                result[i] = pool[index];
                pool.RemoveAt(index);
            }

            return result;
        }

        /// <summary>
        /// 从list中取出num个不同元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static T[] RandomDifferent<T>(IEnumerable<T> list, int num = 1)
        {
            var indexes = RandomDifferent(0, list.Count() - 1, num);
            if (indexes == null) return default;
            var result = new T[num];
            for(int i = 0; i<num; i++)
            {
                result[i] = list.ElementAt(indexes[i]);
            }
            return result;
        }
    }
}
