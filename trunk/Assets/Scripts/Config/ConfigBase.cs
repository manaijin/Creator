using Framework;
using Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creator
{
    public class ConfigBase<T> : Singleton<T> where T : new()
    {
        protected readonly Dictionary<string, ITableItem> data = new Dictionary<string, ITableItem>();

        public bool TryGetItem(string id, out ITableItem item)
        {
            var r = data.TryGetValue(id, out item);
            if (!r) Debug.Log($"{id} is not in {typeof(T).Name}");
            return r;
        }

        public bool TryGetItemValue<T1>(string id, out T1 value)
        {
            value = default;
            if (!TryGetItem(id, out var item)) return false;
            return DeValue(item, out value);
        }

        public void AddItem(string id, ITableItem item)
        {
            try
            {
                data.Add(id, item);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private const string value_name = "value";

        public static bool DeValue<T1>(object item, out T1 value)
        {
            value = default;
            if (item == null) return false;
            var property = ClassUtil.GetPropertyOrFieldValue(item, value_name);
            if (string.IsNullOrEmpty(property)) return false;
            value = JsonUtil.DeserialObject<T1>(property);
            return true;
        }
    }

    public interface ITableItem { }
}
