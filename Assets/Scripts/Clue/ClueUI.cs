using TMPro;
using UnityEngine;

public class ClueUI : UIBase, IView<CluePresenter>
{
    [SerializeField]
    private TMP_Text clueText;

    private CluePresenter presenter;

    public void SetPresenter(CluePresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
    }

    public void OpenClue(string clueId)
    {
        if (!StaticDataHolder.I.TryGetClue(clueId, out var clueData))
        {
            return;
        }

        clueText.text = clueData.Text;
        Show();

        foreach (var wordData in clueData.UnlockWords)
        {
            presenter.AdddWord(wordData.Id);
        }
    }
}
