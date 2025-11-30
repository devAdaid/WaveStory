using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : UIBase
{
    private const string BgmVolumeKey = "BgmVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const float DefaultVolume = 1f;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string bgmVolumeParameter = "BgmVolume";
    [SerializeField] private string sfxVolumeParameter = "SfxVolume";

    [Header("BGM")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TMP_Text bgmValueText;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;

    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxValueText;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    protected override void InitializeInternal()
    {
        AudioManager.I.SetMixerGroup(bgmMixerGroup, sfxMixerGroup);

        StartCoroutine(InitializeInternalCoroutine());
    }

    private IEnumerator InitializeInternalCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        bgmSlider.minValue = 0.0001f;
        bgmSlider.maxValue = 1f;
        sfxSlider.minValue = 0.0001f;
        sfxSlider.maxValue = 1f;

        float bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, DefaultVolume);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultVolume);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        ApplyBgmVolume(bgmVolume);
        ApplySfxVolume(sfxVolume);

        UpdateBgmText(bgmVolume);
        UpdateSfxText(sfxVolume);

        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    private void OnBgmSliderChanged(float value)
    {
        ApplyBgmVolume(value);
        UpdateBgmText(value);
        PlayerPrefs.SetFloat(BgmVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void OnSfxSliderChanged(float value)
    {
        ApplySfxVolume(value);
        UpdateSfxText(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyBgmVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        audioMixer.SetFloat(bgmVolumeParameter, dB);
    }

    private void ApplySfxVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        audioMixer.SetFloat(sfxVolumeParameter, dB);
    }

    private void UpdateBgmText(float value)
    {
        if (bgmValueText)
        {
            bgmValueText.text = Mathf.RoundToInt(value * 100f).ToString();
        }
    }

    private void UpdateSfxText(float value)
    {
        if (sfxValueText)
        {
            sfxValueText.text = Mathf.RoundToInt(value * 100f).ToString();
        }
    }

    public static void LoadAndApplyVolumeSettings(AudioMixer audioMixer, string bgmParam, string sfxParam)
    {
        float bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, DefaultVolume);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultVolume);

        float bgmDb = Mathf.Log10(Mathf.Max(bgmVolume, 0.0001f)) * 20f;
        float sfxDb = Mathf.Log10(Mathf.Max(sfxVolume, 0.0001f)) * 20f;

        audioMixer.SetFloat(bgmParam, bgmDb);
        audioMixer.SetFloat(sfxParam, sfxDb);
    }
}
