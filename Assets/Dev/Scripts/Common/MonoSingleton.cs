using Dacodelaac.Core;
using UnityEngine;

namespace _Root.Scripts.Pattern
{
    public abstract class MonoSingleton<T> : BaseMono where T : MonoSingleton<T>
    {
        private static T m_Instance;
        static bool shuttingDown;

        public static T Instance
        {
            get
            {
                if (m_Instance != null || shuttingDown || !Application.isPlaying) return m_Instance;
                m_Instance = FindObjectOfType(typeof(T)) as T;

                if (m_Instance == null)
                {
                    m_Instance = new GameObject("Temp Instance of " + typeof(T), typeof(T))
                        .GetComponent<T>();
                }

                return m_Instance;
            }
        }

        protected virtual void Awake()
        {
            if (m_Instance == null)
                m_Instance = this as T;
            else if (m_Instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (this == m_Instance)
                m_Instance = null;
        }

        private void OnApplicationQuit()
        {
            m_Instance = null;
            shuttingDown = true;
        }
    }
}