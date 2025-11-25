using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class WordInputUI : UIBase, IView<WordInputPresenter>
{
    [SerializeField]
    private TMP_Text wordText1;

    [SerializeField]
    private TMP_Text wordText2;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private WordInventoryUI inventoryUI;

    [field: SerializeField]
    public LocalizedString SoulUnlockedMessage;
    [field: SerializeField]
    public LocalizedString SoulNotMatchedMessage;
    [field: SerializeField]
    public LocalizedString SoulAlreadyUnlockedMessage;

    private string wordId1;
    private string wordId2;

    private WordInputPresenter presenter;

    private static string EMPTY_TEXT = "_________";

    private LocalizeStringEvent wordText1LocalizeEvent;
    private LocalizeStringEvent wordText2LocalizeEvent;

    public void SetPresenter(WordInputPresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        inventoryUI.SetCallback(OnWordClicked);
    }

    public override void OnShow()
    {
        inventoryUI.Show();
    }

    public override void OnHide()
    {
        inventoryUI.Hide();
    }

    public void ClearAllWords()
    {
        ApplyWords(string.Empty, string.Empty);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsActive)
        {
            Hide();
        }
    }

    private void OnConfirm()
    {
        presenter.ProcessInput(wordId1, wordId2);

        ClearAllWords();
        Hide();
    }

    private void OnWordClicked(string wordId)
    {
        if (!StaticDataHolder.I.TryGetWord(wordId, out var wordData))
        {
            return;
        }

        var newWordId1 = wordId1;
        var newWordId2 = wordId2;

        if (string.IsNullOrEmpty(newWordId1))
        {
            newWordId1 = wordId;
        }
        else if (string.IsNullOrEmpty(newWordId2))
        {
            newWordId2 = wordId;
        }

        ApplyWords(newWordId1, newWordId2);
    }

    private void ApplyWords(string wordId1, string wordId2)
    {
        this.wordId1 = wordId1;
        this.wordId2 = wordId2;

        ApplyWordText(wordText1, wordId1, ref wordText1LocalizeEvent);
        ApplyWordText(wordText2, wordId2, ref wordText2LocalizeEvent);
    }

    private void ApplyWordText(TMP_Text textComponent, string wordId, ref LocalizeStringEvent localizeEvent)
    {
        if (localizeEvent != null)
        {
            Destroy(localizeEvent);
            localizeEvent = null;
        }

        if (string.IsNullOrEmpty(wordId) || !StaticDataHolder.I.TryGetWord(wordId, out var wordData))
        {
            textComponent.text = EMPTY_TEXT;
        }
        else
        {
            TextHelper.SetLocalizedText(textComponent, wordData.DisplayText, ref localizeEvent);
        }
    }
}
