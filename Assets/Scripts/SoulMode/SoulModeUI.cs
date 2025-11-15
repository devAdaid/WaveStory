using UnityEngine;
using UnityEngine.UI;

public class SoulModeUI : UIBase
{
    [SerializeField]
    private Button waveButton;

    [SerializeField]
    private UIBase waveControlUI;

    protected override void InitializeInternal()
    {
        waveButton.onClick.AddListener(waveControlUI.Show);
    }
}
