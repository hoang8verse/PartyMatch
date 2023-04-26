using UnityEngine;

namespace Utility
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType(typeof(T)) as T;
                }

                return m_Instance;
            }
        }

        public static bool HasInstance => m_Instance != null;

        private void OnDestroy()
        {
            //GameLog.Log(LogType.Log, $"Singleton OnDestroy");
            m_Instance = null;
        }
    }
}