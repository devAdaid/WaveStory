using UnityEngine;
using UnityEngine.UI;

public class SoulModeUI : UIBase
{
    [SerializeField]
    private Button waveButton;

    [SerializeField]
    private UIBase waveControlUI;

    private SouldModePresenter presenter;
    private bool isSoulMode;

    protected override void InitializeInternal()
    {
        presenter = new SouldModePresenter(GM.I.SoulMode, this);
        waveButton.onClick.AddListener(waveControlUI.Show);

        Apply(GM.I.SoulMode.IsSoulMode);
    }

    public void Apply(bool isSoulMode)
    {
        this.isSoulMode = isSoulMode;
        if (isSoulMode)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}
