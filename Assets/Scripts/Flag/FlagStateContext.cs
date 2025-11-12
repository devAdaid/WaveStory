using System.Collections.Generic;

public class FlagStateContext
{
    private readonly HashSet<string> flagIds = new HashSet<string>();

    public void SetFlag(string flagId, bool value)
    {
        if (value)
        {
            flagIds.Add(flagId);
        }
        else
        {
            flagIds.Remove(flagId);
        }
    }

    public bool HasFlag(string flagId)
    {
        return flagIds.Contains(flagId);
    }
}
