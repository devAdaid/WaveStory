using System.Collections.Generic;

public enum UnlockConditionType
{
    None,
    HasFlag,
    HasNoFlag,
    LockedSoul,
    UnlockSoul,
    ClearedSoul,
}

public class UnlockState
{
    public Dictionary<string, SoulState> SoulStates;
    public HashSet<string> FlagIds;

    public UnlockState(Dictionary<string, SoulState> states, HashSet<string> flagIds)
    {
        SoulStates = states;
        FlagIds = flagIds;
    }
}

[System.Serializable]
public struct UnlockCondition
{
    public UnlockConditionType Type;
    public string Id;

    public bool IsSatisfiedBy(UnlockState context)
    {
        switch (Type)
        {
            case UnlockConditionType.None:
                return true;
            case UnlockConditionType.HasFlag:
                return context.FlagIds.Contains(Id);
            case UnlockConditionType.HasNoFlag:
                return !context.FlagIds.Contains(Id);
            case UnlockConditionType.LockedSoul:
                {
                    return !context.SoulStates.TryGetValue(Id, out var soulState) || soulState == SoulState.Locked;
                }
            case UnlockConditionType.UnlockSoul:
                {
                    return context.SoulStates.TryGetValue(Id, out var soulState) && soulState == SoulState.Unlocked;
                }
            case UnlockConditionType.ClearedSoul:
                {
                    return context.SoulStates.TryGetValue(Id, out var soulState) && soulState == SoulState.Cleared;
                }
        }

        return false;
    }
}
