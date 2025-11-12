using UnityEngine.Events;

public class WaveContext
{
    public WaveParameter WaveParameter { get; private set; }

    public UnityEvent WaveChanged = new UnityEvent();

    public WaveContext(WaveParameter waveParameter)
    {
        this.WaveParameter = waveParameter;
    }

    public void SetParameter(WaveParameter waveParameter)
    {
        WaveParameter = waveParameter;

        WaveChanged.Invoke();
    }
}
