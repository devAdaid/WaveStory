using UnityEngine;
using UnityEngine.UI;

public class WordInputButton : UIBase
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private UIBase wordInputUI;

    public override void Initialize()
    {
        button.onClick.AddListener(wordInputUI.Show);
    }
}
