using System.Collections.Generic;
using UnityEngine.Events;

public class UnlockContext
{
    public UnityEvent OnFlagAndSoulChanged = new UnityEvent();
    public UnityEvent OnClueChanged = new UnityEvent();

    public readonly HashSet<string> FlagIds = new HashSet<string>();
    private readonly HashSet<string> unlockedSouls = new HashSet<string>();
    private readonly HashSet<string> clearedSouls = new HashSet<string>();
    private readonly HashSet<string> unlockedClues = new HashSet<string>();

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

        OnFlagAndSoulChanged.Invoke();
    }

    public bool HasFlag(string flagId)
    {
        return FlagIds.Contains(flagId);
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
}
