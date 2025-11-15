public class WavePresenter_Title : IPresenter
{
    public WaveParameter WaveParameter => context.WaveParameter;

    private readonly WaveContext context;
    private readonly IWaveUI waveUI;

    public WavePresenter_Title(WaveContext context, IWaveUI waveUI)
    {
        this.context = context;
        this.waveUI = waveUI;
        context.WaveChanged.AddListener(UpdateUI);
    }

    public void SetParamter(WaveParameter param)
    {
        context.SetParameter(param);
    }

    private void UpdateUI()
    {
        waveUI.Apply(context.WaveParameter);
    }
}
