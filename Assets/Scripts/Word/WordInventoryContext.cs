using System.Collections.Generic;

public class WordInventoryContext
{
    public readonly List<string> WordIds = new();

    public bool Add(string wordId)
    {
        if (WordIds.Contains(wordId))
        {
            return false;
        }

        WordIds.Add(wordId);
        return true;
    }
}
