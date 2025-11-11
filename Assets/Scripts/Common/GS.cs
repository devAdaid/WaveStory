using UnityEngine;

public class GS : MonoSingleton<GS>, IMonoSingleton
{
    [SerializeField]
    private UIHolder uiHolder;

    public void Initialize()
    {
    }
}
