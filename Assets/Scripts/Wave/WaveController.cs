using UnityEngine;

public class WaveController
{
    private WaveRenderer waveRenderer;

    private WaveParameter waveParameter;

    public WaveController(WaveRenderer waveRenderer, WaveParameter waveParameter)
    {
        this.waveRenderer = waveRenderer;
        SetParamter(waveParameter);
    }

    public void SetAmplitudeStep(int amplitudeStep)
    {
        var param = waveParameter;
        param.AmplitudeStep = amplitudeStep;
        SetParamter(param);
    }

    public void SetFrequencyStep(int frequencyStep)
    {
        var param = waveParameter;
        param.FrequencyStep = frequencyStep;
        SetParamter(param);
    }

    public void SetWaveTypeByValue(int waveType)
    {
        SetWaveType((WaveType)waveType);
    }

    public void SetWaveType(WaveType waveType)
    {
        var param = waveParameter;
        param.WaveType = waveType;
        SetParamter(param);
    }

    public void SetParamter(WaveParameter param)
    {
        waveParameter = param;

        var ampStep = Mathf.Max(StaticDataHolder.I.WaveConstant.MinAmplitudeStep, param.AmplitudeStep);
        var amplitudeT = (float)Mathf.Max(0f, ((float)ampStep / StaticDataHolder.I.WaveConstant.MaxAmplitudeStep));
        var amplitude = Mathf.Lerp(0f, StaticDataHolder.I.WaveConstant.MaxAmplitude, amplitudeT);
        var freqStep = Mathf.Max(StaticDataHolder.I.WaveConstant.MinFrequencyStep, param.FrequencyStep);
        var frequencyT = (float)Mathf.Max(0f, ((float)freqStep / StaticDataHolder.I.WaveConstant.MaxFrequencyStep));
        var frequency = Mathf.Lerp(0f, StaticDataHolder.I.WaveConstant.MaxFrequency, frequencyT);
        waveRenderer.SetParameter(param.WaveType, amplitude, frequency);
    }

    public void SetPause(bool isPause)
    {
        waveRenderer.SetPause(isPause);
    }
}
