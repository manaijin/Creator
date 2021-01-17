using Framework;
using Framework.UI;
using GameModule;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Framework.InputType;

namespace Creator
{
    public class GameStart : SingletonMono<GameStart>
    {
        [HideInInspector]
        public static LayerBase BottomLayer;
        [HideInInspector]
        public static LayerBase MidLayer;
        [HideInInspector]
        public static LayerBase TopLayer;
        [HideInInspector]
        public static LayerBase HideLayer;

        private static bool m_isInited = false;
        private int m_MainPageMouseCBID;

        private int item_num = 0;

        private IEnumerator Start()
        {
            if (m_isInited) yield return null;
            InitUILayer();
            yield return LoadStartUI();
            yield return BindMouseEvent();
            CreateItem();
            m_isInited = true;
        }

        private void InitUILayer()
        {
            var param = new UIMgrParam();
            GameObject.Find("UI").TryGetComponent(out param.root);
            GameObject.Find("UI/UICamera").TryGetComponent(out param.uiCamera);
            UIMgr.Instance.InitRoot(param);

            BottomLayer = new LayerBase(new LayerParam() { Layer = GameObject.Find("UI/Bottom").GetComponent<RectTransform>() });
            MidLayer = new LayerBase(new LayerParam() { Layer = GameObject.Find("UI/Middle").GetComponent<RectTransform>() });
            TopLayer = new LayerBase(new LayerParam() { Layer = GameObject.Find("UI/Top").GetComponent<RectTransform>() });
            HideLayer = new HideLayer(new LayerParam() { Name = "Hide" });
            UIMgr.Instance.AddLayers(new LayerBase[] { BottomLayer, MidLayer, TopLayer, HideLayer });
        }

        IEnumerator LoadStartUI()
        {
            var stratView = new StartView() { HideNode = HideLayer.Node };
            yield return MidLayer.PushView(stratView);
        }

        IEnumerator BindMouseEvent()
        {
            var result = ResourceMgr.Instance.LoadAssetAsync("cursor_effect_1");
            yield return new WaitUntil(() => { return result.IsDone; });
            PoolMgr.Instance.TryGetPool("cursor_effect_1", out UnityPool pool);
            pool.onGetObjectInPool += (obj) => obj.SetActive(true);
            pool.onPutObject += (obj) => obj.SetActive(false);
            var param = new InputCombination<MouseButton>(MouseButton.leftButton, KeyState.wasPressedThisFrame);
            m_MainPageMouseCBID = InputMgr.Instance.RegistMouseCallBack(param, () => ClickCB(pool));
        }

        void ClickCB(UnityPool pool)
        {
            GameObject go = pool.Get();
            var pos = Mouse.current.position.ReadValue();
            go.transform.SetParent(TopLayer.Node, false);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(TopLayer.Node, pos, UIMgr.Instance.UICamera, out Vector2 relatePos);
            go.transform.GetComponent<RectTransform>().anchoredPosition = relatePos;
        }

        private void Update()
        {
            InputMgr.Instance.Update();
        }

        private void OnDestroy()
        {
            if (m_MainPageMouseCBID != 0)
                InputMgr.Instance.UnregistMouseCallBack(m_MainPageMouseCBID);
        }

        private void CreateItem()
        {
            GlobalConfig.Instance.TryGetItemValue("random_float_num_range", out int[] random_float_num_range);
            GlobalConfig.Instance.TryGetItemValue("random_float_size_range", out int[] random_float_size_range);
            int num = Random.Range(random_float_num_range[0], random_float_num_range[1]);
            print(num);
            for (int i = 0; i < num; i++)
            {
                ResourceMgr.Instance.InstantiateAsync("random_float_1", go =>
                {
                    go.transform.position = Vector3.zero;
                    item_num += 1;
                });
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(30, 10, 100, 200), string.Format($"当前实例化数量：{item_num}"));
        }
    }
}