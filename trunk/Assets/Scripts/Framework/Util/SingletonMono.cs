using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T intance;
        public static T Instance
        {
            get
            {
                return intance;
            }
        }

        protected virtual void Awake() 
        {
            if (intance == null)
                this.TryGetComponent<T>(out intance);
        }
    }
}
