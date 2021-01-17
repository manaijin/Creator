using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// View基类
/// </summary>
namespace Framework.UI
{
    public struct LayerParam
    {
        public string Name;
        public int Order;
        public RectTransform Layer;
    }

    public class LayerBase : System.Object
    {
        /// <summary>
        /// 层级编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 层级名称
        /// </summary>
        public string Name { get { return name; } }
        protected string name;

        /// <summary>
        /// 层级优先级
        /// </summary>
        public int Order { get { return order; } }
        protected int order;

        /// <summary>
        /// 层级的GameObject
        /// </summary>
        public RectTransform Node { get { return node; } }
        protected RectTransform node;

        private readonly List<ViewBase> viewList;
        public LayerBase()
        {
            viewList = new List<ViewBase>();
        }

        protected LayerBase(int order = 0, RectTransform layer = null, string name = "") : this()
        {
            this.order = order;
            node = layer ? layer : new GameObject(name).AddComponent<RectTransform>();
            if (!string.IsNullOrEmpty(name))
                node.name = name;
        }


        public LayerBase(LayerParam param) : this()
        {
            this.order = param.Order;
            node = param.Layer ? param.Layer : new GameObject().AddComponent<RectTransform>();
            if (!string.IsNullOrEmpty(param.Name))
                node.name = param.Name;
        }

        public virtual IEnumerator PushView(ViewBase ui)
        {
            yield return ui.OnEnter();
            ui.LayerNode = node;
            if (ui.View != null)
            {
                ui.View.SetParent(node, false);
            }
            viewList.Add(ui);
        }

        public virtual void PopView(ViewBase ui)
        {
            ui.OnExit();
            viewList.Remove(ui);
        }

        public virtual void ClearLayer()
        {
            node.DetachChildren();
            foreach (var view in viewList)
            {
                view.Hide();
            }
            viewList.Clear();
        }

        public virtual void Destroy()
        {
            node.DetachChildren();
            UnityEngine.Object.Destroy(node.gameObject);
            foreach (var view in viewList)
            {
                view.Destroy();
            }
            viewList.Clear();
        }
    }
}
