using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class Singleton<T> where T : new()
    {
        private static T m_instance;
        private readonly static object m_lock = new object();

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lock)
                    {
                        if(m_instance == null)
                            m_instance = new T();
                    }
                }                
                return m_instance;
            }
        }

        protected Singleton() { }
    }
}
