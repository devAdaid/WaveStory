public class SouldModePresenter
{
    private SoulModeContext soulMode;
    private SoulModeUI ui;

    public SouldModePresenter(SoulModeContext soulMode, SoulModeUI ui)
    {
        this.soulMode = soulMode;
        this.ui = ui;
        soulMode.OnSoulModeChanged.AddListener(OnSoulModeChanged);
    }

    public void ChangeSoulMode(bool isSoulMode)
    {
        if (soulMode.IsSoulMode != isSoulMode)
        {
            soulMode.SetSoulMode(isSoulMode);
        }
    }

    private void OnSoulModeChanged(bool isSoulMode)
    {
        ui.Apply(soulMode.IsSoulMode);
    }
}
