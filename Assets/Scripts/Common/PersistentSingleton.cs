using UnityEngine;

public abstract class PersistentSingleton<T> : MonoSingleton<T> where T : MonoBehaviour, IMonoSingleton
{

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}