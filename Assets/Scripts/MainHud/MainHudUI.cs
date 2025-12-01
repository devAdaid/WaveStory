using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MainHudUI : UIBase
{
    [SerializeField]
    private Button clueButton;

    [SerializeField]
    private Button soulModeButton;

    [SerializeField]
    private Image soulModeButtonImg;
    
    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private Sprite realModeSprite;

    [SerializeField]
    private Sprite soulModeSprite;

    [SerializeField]
    private TMP_Text soulModeText;

    [SerializeField]
    private LocalizedString realModeStr;

    [SerializeField]
    private LocalizedString soulModeStr;

    private MainHudPresenter presenter;
    private bool isSoulMode;
    private LocalizeStringEvent soulModeTextEvent;

    public void SetPresenter(MainHudPresenter presenter)
    {
        this.presenter = presenter;
        isSoulMode = presenter.IsSoulMode;
    }

    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;

        LocalizationSettings.SelectedLocaleChanged -= StartUI.OnLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += StartUI.OnLocaleChanged;

        if (PlayerPrefs.HasKey(StartUI.LanguageCodeKey))
        {
            string savedCode = PlayerPrefs.GetString(StartUI.LanguageCodeKey);
            StartUI.SetLocale(savedCode);
        }
    }

    protected override void InitializeInternal()
    {
        clueButton.onClick.AddListener(OpenClueUI);
        soulModeButton.onClick.AddListener(ToggleSoulMode);
        settingsButton.onClick.AddListener(OpenSettingsUI);
        TextHelper.SetLocalizedTextEvent(soulModeText, realModeStr, ref soulModeTextEvent);
    }

    private void Update()
    {
        var showMainHud = GM.I.UIHolder.IsShowMainHud();

        if (showMainHud && !IsActive)
        {
            Show();
        }
        else if (!showMainHud && IsActive)
        {
            Hide();
        }

        if (IsActive)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                OpenClueUI();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                ToggleSoulMode();
            }
        }
    }

    public void UpdateUI(bool isSoulMode)
    {
        soulModeButtonImg.sprite = isSoulMode ? soulModeSprite : realModeSprite;
        this.isSoulMode = isSoulMode;

        if (soulModeTextEvent)
        {
            soulModeTextEvent.StringReference = isSoulMode ? soulModeStr : realModeStr;
            soulModeTextEvent.RefreshString();
        }
    }

    private void OpenClueUI()
    {
        GM.I.UIHolder.ClueUI.Show();
    }

    private void ToggleSoulMode()
    {
        var newSoulMode = !isSoulMode;
        if (newSoulMode)
        {
            AudioManager.I.PlaySfxOneShot("Vision");
        }
        else
        {
            AudioManager.I.PlaySfxOneShot("VisionOff");
        }

        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(() => presenter.SetSoulMode(newSoulMode));
    }
    
    private void OpenSettingsUI()
    {
        GM.I.UIHolder.SettingsUI.Show();
    }
}
