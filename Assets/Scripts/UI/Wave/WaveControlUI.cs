using UnityEngine;

public class WaveControlUI : UIBase
{
    [SerializeField]
    private WaveRenderer waveRenderer;
    [SerializeField]
    private KnobButton amplitudeStepButton;
    [SerializeField]
    private KnobButton frequencyStepButton;
    [SerializeField]
    private float inputDelay = 0.5f; // 입력 딜레이 (초 단위)

    private WaveController waveController => waveRenderer.WaveController;
    private WaveParameter waveParameter => waveController.WaveParameter;

    // 각 키별 마지막 입력 시간 추적
    private float lastInputTimeA;
    private float lastInputTimeD;
    private float lastInputTimeS;
    private float lastInputTimeW;

    public override void Initialize()
    {
        amplitudeStepButton.Initialize(WaveLogic.MinAmplitudeStep, WaveLogic.MaxAmplitudeStep, waveParameter.AmplitudeStep, OnAmplitudeStepChanged);
        frequencyStepButton.Initialize(WaveLogic.MinFrequencyStep, WaveLogic.MaxFrequencyStep, waveParameter.FrequencyStep, OnFrequencyStepChanged);
    }

    void Update()
    {
        float currentTime = Time.time;

        // A 키: 주파수 감소 (꾹 눌러도 딜레이 적용)
        if (Input.GetKey(KeyCode.A) && currentTime - lastInputTimeA >= inputDelay)
        {
            var newParam = waveParameter;
            newParam.FrequencyStep = WaveLogic.GetClampedFrequenctStep(waveParameter.FrequencyStep - 1);
            ApplyNewWaveParameter(newParam);
            lastInputTimeA = currentTime;
        }
        // D 키: 주파수 증가
        else if (Input.GetKey(KeyCode.D) && currentTime - lastInputTimeD >= inputDelay)
        {
            var newParam = waveParameter;
            newParam.FrequencyStep = WaveLogic.GetClampedFrequenctStep(waveParameter.FrequencyStep + 1);
            ApplyNewWaveParameter(newParam);
            lastInputTimeD = currentTime;
        }
        // S 키: 진폭 감소
        else if (Input.GetKey(KeyCode.S) && currentTime - lastInputTimeS >= inputDelay)
        {
            var newParam = waveParameter;
            newParam.AmplitudeStep = WaveLogic.GetClampedAmplitudeStep(waveParameter.AmplitudeStep - 1);
            ApplyNewWaveParameter(newParam);
            lastInputTimeS = currentTime;
        }
        // W 키: 진폭 증가
        else if (Input.GetKey(KeyCode.W) && currentTime - lastInputTimeW >= inputDelay)
        {
            var newParam = waveParameter;
            newParam.AmplitudeStep = WaveLogic.GetClampedAmplitudeStep(waveParameter.AmplitudeStep + 1);
            ApplyNewWaveParameter(newParam);
            lastInputTimeW = currentTime;
        }

        var isPauseWave = false;
        if (Input.GetKey(KeyCode.Space))
        {
            isPauseWave = true;
        }
        waveController.SetPause(isPauseWave);
    }

    private void OnAmplitudeStepChanged(int amplitudeStep)
    {
        var newParam = waveParameter;
        newParam.AmplitudeStep = amplitudeStep;
        ApplyNewWaveParameter(newParam);
    }

    private void OnFrequencyStepChanged(int frequencyStep)
    {
        var newParam = waveParameter;
        newParam.FrequencyStep = frequencyStep;
        ApplyNewWaveParameter(newParam);
    }

    private void ApplyNewWaveParameter(WaveParameter waveParameter)
    {
        waveController.SetParamter(waveParameter);
        amplitudeStepButton.SetStep(waveParameter.AmplitudeStep);
        frequencyStepButton.SetStep(waveParameter.FrequencyStep);
    }
}