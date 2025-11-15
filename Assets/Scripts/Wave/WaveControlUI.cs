using System.Collections.Generic;
using UnityEngine;

public class WaveControlUI : UIBase
{
    [SerializeField]
    private KnobButton amplitudeStepButton;
    [SerializeField]
    private KnobButton frequencyStepButton;
    [SerializeField]
    private float inputDelay = 0.5f; // 입력 딜레이 (초 단위)
    [SerializeField]
    private WaveRenderer inputWaveRenderer;
    [SerializeField]
    private int previewInitialCount;
    [SerializeField]
    private Transform previewRoot;
    [SerializeField]
    private WaveRenderer previewTemplate;

    // 각 키별 마지막 입력 시간 추적
    private float lastInputTimeA;
    private float lastInputTimeD;
    private float lastInputTimeS;
    private float lastInputTimeW;

    private bool isChangeBlock;

    private WaveControlPresenter presenter;

    private List<WaveRenderer> previewPool = new List<WaveRenderer>();

    public void SetPresenter(WaveControlPresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
        var waveParameter = presenter.WaveParameter;
        amplitudeStepButton.Initialize(WaveLogic.MinAmplitudeStep, WaveLogic.MaxAmplitudeStep, waveParameter.AmplitudeStep, OnAmplitudeStepChanged);
        frequencyStepButton.Initialize(WaveLogic.MinFrequencyStep, WaveLogic.MaxFrequencyStep, waveParameter.FrequencyStep, OnFrequencyStepChanged);

        amplitudeStepButton.SetStep(presenter.WaveParameter.AmplitudeStep);
        frequencyStepButton.SetStep(presenter.WaveParameter.FrequencyStep);

        // buttonRoot에 buttonInitialCount만큼 추가하고 setactive false한다
        for (int i = 0; i < previewInitialCount; i++)
        {
            var button = Instantiate(previewTemplate, previewRoot);
            button.gameObject.SetActive(false);
            previewPool.Add(button);
        }

        // 템플릿은 비활성화
        previewTemplate.gameObject.SetActive(false);

        presenter.UpdateUI();
    }

    void Update()
    {
        if (!isInitialized) return;

        float currentTime = Time.time;

        var newParam = presenter.WaveParameter;

        // A 키: 주파수 감소 (꾹 눌러도 딜레이 적용)
        if (Input.GetKey(KeyCode.A) && currentTime - lastInputTimeA >= inputDelay)
        {
            newParam.FrequencyStep = WaveLogic.GetClampedFrequenctStep(newParam.FrequencyStep - 1);
            lastInputTimeA = currentTime;
        }
        // D 키: 주파수 증가
        else if (Input.GetKey(KeyCode.D) && currentTime - lastInputTimeD >= inputDelay)
        {
            newParam.FrequencyStep = WaveLogic.GetClampedFrequenctStep(newParam.FrequencyStep + 1);
            lastInputTimeD = currentTime;
        }
        // S 키: 진폭 감소
        else if (Input.GetKey(KeyCode.S) && currentTime - lastInputTimeS >= inputDelay)
        {
            newParam.AmplitudeStep = WaveLogic.GetClampedAmplitudeStep(newParam.AmplitudeStep - 1);
            lastInputTimeS = currentTime;
        }
        // W 키: 진폭 증가
        else if (Input.GetKey(KeyCode.W) && currentTime - lastInputTimeW >= inputDelay)
        {
            newParam.AmplitudeStep = WaveLogic.GetClampedAmplitudeStep(newParam.AmplitudeStep + 1);
            lastInputTimeW = currentTime;
        }

        if (newParam != presenter.WaveParameter)
        {
            OnInput(newParam);
        }
    }

    public void SetChangeBlock(bool isChangeBlock)
    {
        this.isChangeBlock = isChangeBlock;
        amplitudeStepButton.SetChangeBlock(isChangeBlock);
        frequencyStepButton.SetChangeBlock(isChangeBlock);
    }

    private void OnAmplitudeStepChanged(int amplitudeStep)
    {
        var newParam = presenter.WaveParameter;
        newParam.AmplitudeStep = amplitudeStep;
        OnInput(newParam);
    }

    private void OnFrequencyStepChanged(int frequencyStep)
    {
        var newParam = presenter.WaveParameter;
        newParam.FrequencyStep = frequencyStep;
        OnInput(newParam);
    }

    private void OnInput(WaveParameter waveParameter)
    {
        if (isChangeBlock)
        {
            return;
        }

        presenter.SetParamter(waveParameter);
    }

    public void Apply(WaveParameter inputParameter, List<WaveParameter> previewParameters)
    {
        inputWaveRenderer.Apply(inputParameter);
        amplitudeStepButton.SetStep(presenter.WaveParameter.AmplitudeStep, true);
        frequencyStepButton.SetStep(presenter.WaveParameter.FrequencyStep, true);

        ApplyPreview(previewParameters);
    }

    public void ApplyPreview(List<WaveParameter> previewParameters)
    {
        // wordIds보다 부족한 개수만큼 button을 추가로 만든다
        int neededCount = previewParameters.Count;
        while (previewPool.Count < neededCount)
        {
            var preview = Instantiate(previewTemplate, previewRoot);
            preview.gameObject.SetActive(false);
            previewPool.Add(preview);
        }

        // wordIds에 해당하는 preview들을 Apply하고 활성화
        for (int i = 0; i < previewParameters.Count; i++)
        {
            previewPool[i].Apply(previewParameters[i]);
            previewPool[i].gameObject.SetActive(true);
        }

        // 나머지 버튼은 setactive false한다
        for (int i = previewParameters.Count; i < previewPool.Count; i++)
        {
            previewPool[i].gameObject.SetActive(false);
        }
    }
}
