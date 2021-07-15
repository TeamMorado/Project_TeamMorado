using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public void Awake()
    {
        lock (m_Lock)
        {
            if (m_Instance == null)
            {
                m_Instance = this.GetComponent<T>();
                DontDestroyOnLoad(m_Instance);
                Setup();
            }
        }
    }

    protected virtual void Setup()
    {

    }

    private static object m_Lock = new object();
    private static T m_Instance;
    public static T Instance
    {
        get
        {

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    if (m_Instance == null)
                    {
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return m_Instance;
            }
        }
    }
}
