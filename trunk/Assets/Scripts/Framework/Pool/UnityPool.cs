using Creator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    public class UnityPool : Pool<GameObject>
    {
        /// <summary>
        /// UnityPool根节点
        /// </summary>
        public static GameObject Root
        {
            get
            {
                if (!root)
                {
                    root = new GameObject("Pool");
                    root.AddComponent<CustomDonotDestory>();
                }
                return root;
            }
        }
        private static GameObject root;

        /// <summary>
        /// 单个对象池存储节点
        /// </summary>
        public Transform Node { get => node; }
        private Transform node;

        public UnityPool(string key) : base(key)
        {
            InitPoolNode();

            onPutObject = (obj) =>
            {
                if (!node) return;
                obj.transform.SetParent(node);
            };
        }

        private void InitPoolNode()
        {
            node = Root.transform.Find(Key);
            if (!node)
            {
                var go = new GameObject(Key);
                node = go.transform;
                node.SetParent(Root.transform);
            }
        }

        public override void Put(GameObject obj)
        {
            if (queue.Contains(obj)) return;
            base.Put(obj);
        }

        public override void OnCreateObject(out GameObject obj)
        {
            obj = ResourceMgr.Instance.Instantiate(Key);
        }

        public override void Clear()
        {
            var count = queue.Count;
            for(var i = 0; i < count; i++)
            {
                var go = queue.Dequeue();
                GameObject.Destroy(go);
            }
            GameObject.Destroy(Node);
            queue.Clear();
        }
    }
}
