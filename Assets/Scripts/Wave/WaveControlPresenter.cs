using System.Linq;

public class WaveControlPresenter : IPresenter
{
    public WaveParameter WaveParameter => wave.WaveParameter;

    private readonly WaveContext wave;
    private readonly RoomContext room;
    private readonly WaveControlUI waveUI;

    public WaveControlPresenter(WaveContext inputWave, RoomContext room, WaveControlUI waveUI)
    {
        this.wave = inputWave;
        this.room = room;
        this.waveUI = waveUI;
        inputWave.WaveChanged.AddListener(UpdateUI);
    }

    public void SetParamter(WaveParameter param)
    {
        wave.SetParameter(param);
    }

    public void UpdateUI()
    {
        waveUI.Apply(wave.WaveParameter, room.PreviewWaves.ToList());
    }
}
