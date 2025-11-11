using UnityEngine;

public class WaveController
{
    private WaveRenderer waveRenderer;

    public WaveParameter WaveParameter => waveParameter;
    private WaveParameter waveParameter;

    private bool isChangeBlock;

    public WaveController(WaveRenderer waveRenderer)
    {
        this.waveRenderer = waveRenderer;
    }

    public void SetChangeBlock(bool isChangeBlock)
    {
        this.isChangeBlock = isChangeBlock;
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
        if (isChangeBlock)
        {
            return;
        }

        waveParameter = param;

        var ampStep = Mathf.Max(WaveLogic.MinAmplitudeStep, param.AmplitudeStep);
        var amplitudeT = (float)Mathf.Max(0f, ((float)ampStep / WaveLogic.MaxAmplitudeStep));
        var amplitude = Mathf.Lerp(0f, StaticDataHolder.I.WaveConstant.MaxAmplitude, amplitudeT);
        var freqStep = Mathf.Max(WaveLogic.MinFrequencyStep, param.FrequencyStep);
        var frequencyT = (float)Mathf.Max(0f, ((float)freqStep / WaveLogic.MaxFrequencyStep));
        var frequency = Mathf.Lerp(0f, StaticDataHolder.I.WaveConstant.MaxFrequency, frequencyT);
        waveRenderer.SetParameter(param.WaveType, amplitude, frequency);
    }

    public void SetPause(bool isPause)
    {
        waveRenderer.SetPause(isPause);
    }
}
