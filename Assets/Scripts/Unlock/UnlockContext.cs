using System.Collections.Generic;
using UnityEngine.Events;

public class UnlockContext
{
    public UnityEvent OnChanged = new UnityEvent();

    public readonly HashSet<string> FlagIds = new HashSet<string>();
    public readonly HashSet<string> UnlockedSouls = new HashSet<string>();

    public void UnlockFlag(string flagId)
    {
        SetFlag(flagId, true);
    }

    public void SetFlag(string flagId, bool value)
    {
        if (value)
        {
            FlagIds.Add(flagId);
        }
        else
        {
            FlagIds.Remove(flagId);
        }

        OnChanged.Invoke();
    }

    public bool HasFlag(string flagId)
    {
        return FlagIds.Contains(flagId);
    }

    public void UnlockSoul(string soulId)
    {
        UnlockedSouls.Add(soulId);

        OnChanged.Invoke();
    }

    public bool IsUnlockedSoul(string soulId)
    {
        return UnlockedSouls.Contains(soulId);
    }
}
