using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Util
{
    public static class ClassUtil
    {
        /// <summary>
        /// 获取类的字段或属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="pro"></param>
        /// <returns></returns>
        public static string GetPropertyOrFieldValue<T>(T item, string pro)
        {
            var t = item.GetType();

            var field = t.GetField(pro);
            if (field != null) return field.GetValue(item) as string;

            var property = t.GetProperty(pro);
            if (property != null) return property.GetValue(item, null) as string;

            return default;
        }
    }
}
