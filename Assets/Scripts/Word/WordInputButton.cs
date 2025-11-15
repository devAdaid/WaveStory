using UnityEngine;
using UnityEngine.UI;

public class WordInputButton : UIBase
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private UIBase wordInputUI;

    protected override void InitializeInternal()
    {
        button.onClick.AddListener(wordInputUI.Show);
        button.onClick.AddListener(() => AudioManager.I.PlaySfxOneShot("Select"));
    }
}
