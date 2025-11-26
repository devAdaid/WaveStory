using System.Collections.Generic;
using UnityEngine.Events;

public class WordInventoryContext
{
    private readonly Dictionary<int, List<string>> wordsByFloor;
    public UnityEvent<string> OnWordAdded = new();

    public WordInventoryContext(Dictionary<int, List<string>> wordsByFloor)
    {
        this.wordsByFloor = wordsByFloor;
    }

    public bool Add(string wordId, int floor)
    {
        if (!wordsByFloor.TryGetValue(floor, out var wordIds))
        {
            wordIds = new List<string>();
            wordsByFloor.Add(floor, wordIds);
        }

        if (wordIds.Contains(wordId))
        {
            return false;
        }

        wordIds.Add(wordId);
        OnWordAdded.Invoke(wordId);
        return true;
    }

    public List<string> GetWords(int floor)
    {
        if (wordsByFloor.TryGetValue(floor, out var words))
        {
            return words;
        }

        return new List<string>();
    }
}
