using System.Collections.Generic;
using UnityEngine.Events;

public class WordInventoryContext
{
    public readonly List<string> WordIds = new();
    public UnityEvent<string> OnWordAdded = new();

    public WordInventoryContext(List<string> wordIds)
    {
        WordIds = wordIds;
    }

    public bool Add(string wordId)
    {
        if (WordIds.Contains(wordId))
        {
            return false;
        }

        WordIds.Add(wordId);
        OnWordAdded.Invoke(wordId);
        return true;
    }
}
