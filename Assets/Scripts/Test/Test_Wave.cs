using UnityEngine;

public class Test_Wave : MonoBehaviour
{
    public WaveRenderer WR;
    public WaveController WC;

    private void Start()
    {
        WC = new WaveController(WR, GetRandomWave());
    }

    public WaveParameter GetRandomWave()
    {
        var type = (WaveType)Random.Range((int)WaveType.Sin, (int)WaveType.Count);
        var ampStep = (Random.Range(StaticDataHolder.I.WaveConstant.MinAmplitudeStep, StaticDataHolder.I.WaveConstant.MaxAmplitudeStep + 1));
        var freqStep = (Random.Range(StaticDataHolder.I.WaveConstant.MinFrequencyStep, StaticDataHolder.I.WaveConstant.MaxFrequencyStep + 1));
        return new WaveParameter(type, ampStep, freqStep);
    }
}
