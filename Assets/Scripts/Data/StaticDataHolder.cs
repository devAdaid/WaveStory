using System.Collections.Generic;
using UnityEngine;

public class StaticDataHolder : PersistentSingleton<StaticDataHolder>, IMonoSingleton
{
    public WaveConstant WaveConstant;
    public Dictionary<string, WordData> WordMap = new();

    public void Initialize()
    {
        WaveConstant = Resources.Load<WaveConstant>("Data/WaveConstant");

        var words = Resources.LoadAll<WordData>("Data/Words");
        foreach (var word in words)
        {
            WordMap[word.Id] = word;
        }
    }

    public bool TryGetWord(string id, out WordData word)
    {
        return WordMap.TryGetValue(id, out word);
    }
}
