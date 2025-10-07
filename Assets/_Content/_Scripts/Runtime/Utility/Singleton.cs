using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual bool ShouldPersist => false; // Override this in derived class to control persistence

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            if (ShouldPersist)
                DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Additional initialization if needed
        Initialize();
    }

    protected virtual void Initialize()
    {
        // Override this for any singleton-specific initialization
    }

    protected virtual void OnDestroy()
    {
        // Only nullify the instance if this is the current instance being destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }
}