using Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public struct UIMgrParam
    {

        public RectTransform root;
        public Camera uiCamera;
    }

    class UIMgr : Singleton<UIMgr>
    {
        public Camera UICamera
        {
            get => uiCamera;
        }
        private static readonly List<LayerBase> layerBuffer;
        private static RectTransform root;
        private static Camera uiCamera;

        static UIMgr()
        {
            layerBuffer = new List<LayerBase>();
        }

        public void InitRoot(UIMgrParam param)
        {
            Canvas canvas;
            if (param.root)
            {
                root = param.root;
                root.gameObject.TryGetComponent<Canvas>(out canvas);
            }
            else
            {
                root = param.root ? param.root : new GameObject("UI").AddComponent<RectTransform>();
                canvas = root.gameObject.AddComponent<Canvas>();
                root.gameObject.AddComponent<CanvasScaler>();
                root.gameObject.AddComponent<GraphicRaycaster>();
            }

            uiCamera = param.uiCamera ? param.uiCamera : new GameObject("UICamera").AddComponent<Camera>();
            uiCamera.transform.SetParent(root, false);
            uiCamera.transform.localPosition = new Vector3(0, 0, -100);
            uiCamera.orthographic = true;

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;
        }

        public void AddLayers(LayerBase[] layers)
        {
            foreach (var layer in layers)
            {
                AddLayer(layer);
            }
        }

        public void AddLayer(LayerBase layer)
        {
            var count = layerBuffer.Count;
            layer.Node.SetParent(root, false);
            UIUtil.SetRectFullSize(layer.Node);
            layer.ID = count;
            layerBuffer.Add(layer);
        }

        public void Destroy()
        {
            foreach (var layer in layerBuffer)
            {
                layer.Destroy();
            }
            layerBuffer.Clear();
        }
    }
}
