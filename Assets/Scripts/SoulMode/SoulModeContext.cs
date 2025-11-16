using UnityEngine.Events;

public class SoulModeContext
{
    public bool IsSoulMode { get; private set; }

    public UnityEvent<bool> OnSoulModeChanged = new();

    public void SetSoulMode(bool isSoulMode)
    {
        IsSoulMode = isSoulMode;
        OnSoulModeChanged.Invoke(isSoulMode);
    }
}
