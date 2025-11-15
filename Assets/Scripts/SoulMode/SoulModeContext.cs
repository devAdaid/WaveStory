using UnityEngine.Events;

public class SoulModeContext
{
    public bool IsSoulMode { get; private set; }

    public UnityEvent SoulModeChanged = new UnityEvent();

    public void SetSoulMode(bool isSoulMode)
    {
        IsSoulMode = isSoulMode;
        SoulModeChanged.Invoke();
    }
}
