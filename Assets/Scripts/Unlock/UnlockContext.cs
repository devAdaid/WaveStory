using System.Collections.Generic;
using UnityEngine.Events;

public class UnlockContext
{
    public UnityEvent OnFlagAndSoulChanged = new UnityEvent();
    public UnityEvent OnClueChanged = new UnityEvent();

    public HashSet<string> FlagIds => flagIds;

    private readonly HashSet<string> flagIds = new HashSet<string>();
    private readonly HashSet<string> unlockedSouls = new HashSet<string>();
    private readonly HashSet<string> clearedSouls = new HashSet<string>();
    private readonly HashSet<string> unlockedClues = new HashSet<string>();

    public UnlockContext(HashSet<string> flagIds, HashSet<string> unlockedSouls, HashSet<string> clearedSouls, HashSet<string> unlockedClues)
    {
        this.flagIds = flagIds;
        this.unlockedSouls = unlockedSouls;
        this.clearedSouls = clearedSouls;
        this.unlockedClues = unlockedClues;
    }

    public void UnlockFlag(string flagId)
    {
        SetFlag(flagId, true);
    }

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

        OnFlagAndSoulChanged.Invoke();
    }

    public bool HasFlag(string flagId)
    {
        return flagIds.Contains(flagId);
    }

    public void UnlockSoul(string soulId)
    {
        unlockedSouls.Add(soulId);

        OnFlagAndSoulChanged.Invoke();
    }

    public void ClearSoul(string soulId)
    {
        clearedSouls.Add(soulId);

        OnFlagAndSoulChanged.Invoke();
    }

    public UnlockState GetUnlockState()
    {
        var states = new Dictionary<string, SoulState>();

        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            states[soulId] = SoulState.Locked;
        }

        foreach (var soulId in unlockedSouls)
        {
            states[soulId] = SoulState.Unlocked;
        }

        foreach (var soulId in clearedSouls)
        {
            states[soulId] = SoulState.Cleared;
        }

        return new UnlockState(states, FlagIds);
    }

    public void UnlockClue(string clueId)
    {
        unlockedClues.Add(clueId);

        OnClueChanged.Invoke();
    }

    public bool IsUnlockedClue(string clueId)
    {
        return unlockedClues.Contains(clueId);
    }

    public bool IsUnlockedSoul(string soulId)
    {
        return unlockedSouls.Contains(soulId);
    }

    public bool IsClearedSoul(string soulId)
    {
        return clearedSouls.Contains(soulId);
    }
}
