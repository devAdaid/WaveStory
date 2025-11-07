using UnityEngine;

public interface IMonoSingleton
{
    void Initialize();
}

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour, IMonoSingleton
{
    public static bool IsInitialized => _instance != null;

    public static T I
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    T prefab = Resources.Load<T>("Singleton/" + typeof(T).ToString());
                    if (prefab != null)
                    {
                        _instance = Instantiate(prefab) as T;
                        _instance.name = typeof(T).ToString();
                    }
                    else
                    {
                        _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }
                }
            }

            return _instance;
        }
    }

    protected static T _instance = null;
    protected bool isDestroying;

    public void EchoForCreate() { }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            DestroyImmediate(gameObject);
            isDestroying = true;
            return;
        }

        _instance.Initialize();
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}