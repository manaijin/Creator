using Framework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Creator
{
    public class GlobalItem : ITableItem
	{
		public string id;
		public string value;
	}

	public class GlobalConfig: ConfigBase<GlobalConfig>
	{
		public GlobalConfig()
		{
			var json_str = FileUtil.ReadFileToString(Application.dataPath + "/Resource_local/Config/GlobalConfig");
			JObject jo = (JObject)JsonConvert.DeserializeObject(json_str);
			var items = jo.Properties();
			foreach (var item in items)
				AddItem(item.Name, item.Value.ToObject<GlobalItem>());
		}
	}
}