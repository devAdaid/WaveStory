using System.Collections.Generic;
using UnityEngine.Events;

public class UnlockContext
{
    public UnityEvent OnChanged = new UnityEvent();

    public readonly HashSet<string> FlagIds = new HashSet<string>();
    private readonly HashSet<string> unlockedSouls = new HashSet<string>();
    private readonly HashSet<string> clearedSouls = new HashSet<string>();

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
        unlockedSouls.Add(soulId);

        OnChanged.Invoke();
    }

    public void ClearSoul(string soulId)
    {
        clearedSouls.Add(soulId);

        OnChanged.Invoke();
    }

    public Dictionary<string, SoulState> GetSoulStates()
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

        return states;
    }

    public bool SatisfyCondition(UnlockCondition condition)
    {
        switch (condition.Type)
        {
            case UnlockConditionType.None:
                return true;
            case UnlockConditionType.HasFlag:
                return FlagIds.Contains(condition.Id);
            case UnlockConditionType.HasNoFlag:
                return !FlagIds.Contains(condition.Id);
            case UnlockConditionType.UnlockSoul:
                return unlockedSouls.Contains(condition.Id);
            case UnlockConditionType.NotUnlockSoul:
                return !unlockedSouls.Contains(condition.Id);
        }

        return false;
    }
}
