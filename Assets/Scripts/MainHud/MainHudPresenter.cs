public class MainHudPresenter
{
    public bool IsSoulMode => context.IsSoulMode;

    private readonly SoulModeContext context;
    private readonly MainHudUI ui;

    public MainHudPresenter(SoulModeContext context, MainHudUI ui)
    {
        this.context = context;
        this.ui = ui;

        ui.UpdateUI(context.IsSoulMode);
    }

    public void SetSoulMode(bool isSoulMode)
    {
        context.SetSoulMode(isSoulMode);
        ui.UpdateUI(context.IsSoulMode);
    }
}
