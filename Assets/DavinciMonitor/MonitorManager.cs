using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorManager : MonoBehaviour
{
    private static MonitorManager m_instance;
    public static MonitorManager Instance { get { return m_instance; } }

    List<IModel> m_allModels = new List<IModel>();

    void Awake()
    {
        m_instance = this;

        Type[] allTypes = typeof(IModel).Assembly.GetTypes();
        for (int i = 0; i < allTypes.Length; ++i)
            if (allTypes[i].IsSubclassOf(typeof(IModel)))
                m_allModels.Add(Activator.CreateInstance(allTypes[i]) as IModel);

        for (int i = 0; i < m_allModels.Count; ++i)
            m_allModels[i].Init();
    }

    void Update()
    {
        for (int i = 0; i < m_allModels.Count; ++i)
            m_allModels[i].Update();
    }

    public T GetModel<T>() where T : IModel
    {
        for (int i = 0; i < m_allModels.Count; ++i)
            if (m_allModels[i].GetType().Equals(typeof(T)))
                return m_allModels[i] as T;
        return null;
    }
}

public abstract class IModel
{
    public abstract void Init();
    public abstract void Update();
}
