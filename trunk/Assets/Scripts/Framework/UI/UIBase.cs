using System;
using System.Collections;
using UnityEngine;

namespace Framework.UI
{
    public abstract class UIBase
    {
        public string Address { get; set; }

        public RectTransform Root;

        public UIBase()
        {

        }

        public void CreatUI(Action<GameObject> cb = null, Transform parent = null)
        {
            if (string.IsNullOrEmpty(Address))
                Debug.LogError("Address is null");

            if (Root != null)
                Debug.LogError("Root is exist");

            ResourceMgr.Instance.InstantiateAsync(Address, (go) =>
            {
                go.TryGetComponent<RectTransform>(out Root);
                Root.SetParent(parent, false);
                cb?.Invoke(go);
            });
        }

        public AsyncResult CreatUIAsync(Action<GameObject> cb = null, Transform parent = null)
        {
            AsyncResult result = new AsyncResult();
            if (string.IsNullOrEmpty(Address))
            {
                Debug.LogError("Address is null");
                result.IsDone = true;
                return result;
            }

            if (Root != null)
            {
                Debug.LogError("Root is exist");
                result.IsDone = true;
                return result;
            }

            result = ResourceMgr.Instance.InstantiateAsync(Address, (go) =>
            {
                go.TryGetComponent<RectTransform>(out Root);
                Root.SetParent(parent, false);
                cb?.Invoke(go);
            });
            return result;
        }       

        public void Destory()
        {
            GameObject.Destroy(Root.gameObject);
        }

        public abstract AsyncResult CreateTemplate();
    }
}
