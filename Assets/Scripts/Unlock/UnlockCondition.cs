using System.Collections.Generic;

public enum UnlockConditionType
{
    None,
    HasFlag,
    HasNoFlag,
    NotUnlockSoul,
    UnlockSoul,
}

[System.Serializable]
public struct UnlockCondition
{
    public UnlockConditionType Type;
    public string Id;

    public bool SatisfyCondition(Dictionary<string, SoulState> soulStates, HashSet<string> flagIds)
    {
        switch (Type)
        {
            case UnlockConditionType.None:
                return true;
            case UnlockConditionType.HasFlag:
                return flagIds.Contains(Id);
            case UnlockConditionType.HasNoFlag:
                return !flagIds.Contains(Id);
            case UnlockConditionType.UnlockSoul:
                {
                    return soulStates.TryGetValue(Id, out var soulState) && soulState != SoulState.Locked;
                }
            case UnlockConditionType.NotUnlockSoul:
                {
                    return !soulStates.TryGetValue(Id, out var soulState) || soulState == SoulState.Locked;
                }
        }

        return false;
    }
}
