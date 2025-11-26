using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
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

    protected override void InitializeInternal()
    {
        clueButton.onClick.AddListener(OpenClueUI);
        soulModeButton.onClick.AddListener(ToggleSoulMode);
        TextHelper.SetLocalizedText(soulModeText, realModeStr, ref soulModeTextEvent);
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
        presenter.SetSoulMode(!isSoulMode);
    }
}
