using System.Collections.Generic;

public class WordInventoryContext
{
    private readonly HashSet<string> wordIds = new();

    public bool Add(string wordId)
    {
        return wordIds.Add(wordId);
    }
}
