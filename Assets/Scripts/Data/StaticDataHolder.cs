using UnityEngine;

public class StaticDataHolder : PersistentSingleton<StaticDataHolder>, IMonoSingleton
{
    public WaveConstant WaveConstant;

    public void Initialize()
    {
        WaveConstant = Resources.Load<WaveConstant>("Data/WaveConstant");
    }
}
