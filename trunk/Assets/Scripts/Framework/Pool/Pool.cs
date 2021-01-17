using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class Pool<T> : IPool
    {
        /// <summary>
        /// 节点
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 最大容量
        /// </summary>
        public virtual int MaxCount
        {
            get => m_MaxCount;
            set => m_MaxCount = value;
        }
        private int m_MaxCount = -1;

        /// <summary>
        /// 缓存池
        /// </summary>
        protected Queue<T> queue;

        /// <summary>
        /// 回调：从对象池中获取对象时
        /// </summary>
        public Action<T> onGetObjectInPool;

        /// <summary>
        /// 回调：对象放入对象池时
        /// </summary>
        public Action<T> onPutObject;

        private Pool() { }

        public Pool(string key)
        {
            queue = new Queue<T>();
            Key = key;
            PoolMgr.Instance.PutPool(key, this);
        }

        public virtual T Get()
        {
            T obj;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
                OnGetObjectInPool(obj);
            }
            else
                OnCreateObject(out obj);
            return obj;
        }

        public virtual void Put(T obj)
        {
            if (MaxCount > 0 && queue.Count >= MaxCount) return;
            queue.Enqueue(obj);
            OnPutObject(obj);
        }

        /// <summary>
        /// 当从缓存池中抽取对象
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnGetObjectInPool(T obj) { onGetObjectInPool?.Invoke(obj); }

        /// <summary>
        /// 将对象放入缓存池中
        /// </summary>
        /// <param name="obj"></param>
        public virtual void OnPutObject(T obj) { onPutObject?.Invoke(obj); }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <returns></returns>
        public virtual void OnCreateObject(out T obj)
        {
            obj = Activator.CreateInstance<T>();
        }

        public virtual void Clear()
        {
            queue.Clear();
        }
    }

    public interface IPool
    {
        void Clear();
    }
}
