using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    class PoolMgr : Singleton<PoolMgr>
    {
        private static readonly Dictionary<string, IPool> m_Pools;

        static PoolMgr()
        {
            m_Pools = new Dictionary<string, IPool>();
        }

        public void PutPool<T>(string key, Pool<T> pool)
        {
            if (m_Pools.ContainsKey(key)) return;
            m_Pools.Add(key, pool);
        }

        public Pool<object> GetPool(string key)
        {
            if (!m_Pools.ContainsKey(key)) return null;
            return m_Pools[key] as Pool<object>;
        }

        public void TryGetPool<T>(string key, out Pool<T> pool)
        {
            
            if (m_Pools.ContainsKey(key))
            {
                pool = m_Pools[key] as Pool<T>;
            }
            else
            {
                pool = new Pool<T>(key);
            }
        }

        public void TryGetPool(string key, out UnityPool pool)
        {
            if (m_Pools.ContainsKey(key))
            {
                pool = m_Pools[key] as UnityPool;
            }
            else
            {
                pool = new UnityPool(key);
            }
        }

        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAll()
        {
            Debug.LogError("ClearAll");
            foreach (var pool in m_Pools.Values)
            {
                pool.Clear();
            }
            m_Pools.Clear();
        }
    }
}
