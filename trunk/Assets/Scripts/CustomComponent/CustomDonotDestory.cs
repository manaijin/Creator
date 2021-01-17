using System.Collections.Generic;
using UnityEngine;

namespace Creator
{
    public class CustomDonotDestory : MonoBehaviour
    {
        private static readonly List<string> m_NameList = new List<string>();

        private void Awake()
        {
            if (m_NameList.Contains(gameObject.name))
                Destroy(gameObject);
            else
            {
                m_NameList.Add(gameObject.name);
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}