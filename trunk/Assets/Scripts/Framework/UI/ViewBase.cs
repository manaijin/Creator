using Framework.Util;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 视图基类
/// 1.基础功能：创建、销毁、隐藏
/// 2.生命周期：OnEnter、OnPause、OnResume、OnExit
/// </summary>
namespace Framework.UI
{
    public abstract class ViewBase
    {
        public RectTransform View { get { return view; } }
        private RectTransform view;

        public RectTransform HideNode { get; set; }

        public RectTransform LayerNode { get; set; }

        public ViewBase()
        {
            view = new GameObject().AddComponent<RectTransform>();
            UIUtil.SetRectFullSize(view);
        }

        /// <summary>
        /// 进入Layer
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator OnEnter();

        /// <summary>
        /// 离开Layer
        /// </summary>
        public virtual void OnExit() { Destroy(); }

        /// <summary>
        /// 暂停UI功能
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        /// 恢复UI功能
        /// </summary>
        public virtual void OnResume() { }

        public virtual void Destroy()
        {
            UnityEngine.Object.Destroy(view.gameObject);
            LayerNode = null;
            HideNode = null;
        }

        public virtual void Show()
        {
            if (view)
                view.SetParent(LayerNode, false);
        }

        public virtual void Hide()
        {
            if (view)
                view.SetParent(HideNode, false);
        }

        public virtual IEnumerator CreatureUI<T>(Action<T> cb, RectTransform parent = null) where T : UIBase, new()
        {
            var ui = new T();
            var result = ui.CreateTemplate();
            yield return new WaitUntil(() => { return result.IsDone; });
            ui.Root.SetParent(parent ? parent : View);
            if (cb != null && ui != null)
                cb(ui);
        }
    }
}
