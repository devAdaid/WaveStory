using UnityEngine;
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

    private MainHudPresenter presenter;
    private bool isSoulMode;

    public void SetPresenter(MainHudPresenter presenter)
    {
        this.presenter = presenter;
        isSoulMode = presenter.IsSoulMode;
    }

    protected override void InitializeInternal()
    {
        clueButton.onClick.AddListener(OpenClueUI);
        soulModeButton.onClick.AddListener(ToggleSoulMode);
    }

    private void Update()
    {
        if (PopupHandler.I.IsAnyPopup())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenClueUI();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleSoulMode();
        }
    }

    public void UpdateUI(bool isSoulMode)
    {
        soulModeButtonImg.sprite = isSoulMode ? soulModeSprite : realModeSprite;
        this.isSoulMode = isSoulMode;
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
