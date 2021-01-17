using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Util
{
    /// <summary>
    /// 迭代器工具
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// 列表差集
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IEnumerable DifferenceSet<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            var result = new List<T>();
            result.AddRange(a.Except(b));
            result.AddRange(b.Except(a));
            return result;
        }

        /// <summary>
        /// 列表是否包含相同元素
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool ContainRepeatedElement<T>(this IEnumerable<T> list) 
        {
            return ContainRepeatedElement<T>(list, out _);
        }

        /// <summary>
        /// 列表是否包含相同元素,并返回各元素数量
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="repeat">元素数量</param>
        /// <returns></returns>
        public static bool ContainRepeatedElement<T>(this IEnumerable<T> list, out Dictionary<T, int> repeat)
        {
            var result = false;
            repeat = new Dictionary<T, int>();
            if (list == null)
                return result;

            foreach (var item in list)
            {
                if (repeat.ContainsKey(item))
                {
                    repeat[item]++;
                    result = true;
                }
                else
                {
                    repeat.Add(item, 1);
                }
            }
            return result;
        }

        public static string ToString(this IEnumerable enumer)
        {
            StringBuilder s = new StringBuilder();
            foreach (var item in enumer) 
            {
                s.Append(item.ToString());
                s.Append('\n');
            }
            return s.ToString();
        }
    }
}
