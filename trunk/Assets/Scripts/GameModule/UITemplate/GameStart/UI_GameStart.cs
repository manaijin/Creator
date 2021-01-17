using Framework;
using Framework.UI;

namespace GameModule
{
    public partial class UI_GameStart : UIBase
    {
		public UnityEngine.UI.Image bg;
		public UnityEngine.UI.Text title;
		public UnityEngine.Transform btnList;
		public UnityEngine.UI.Button start;
		public UnityEngine.UI.Button hide;
        
        public override AsyncResult CreateTemplate()
        {
            Address = "UI_GameStart";
            var result = CreatUIAsync((obj)=>{
				Root.Find("img_bg/").TryGetComponent(out bg);
				Root.Find("txt_title/").TryGetComponent(out title);
				Root.Find("node_btnList/").TryGetComponent(out btnList);
				Root.Find("node_btnList/btn_start/").TryGetComponent(out start);
				Root.Find("node_btnList/btn_hide/").TryGetComponent(out hide);
            });
            return result;
        }
    }
}
