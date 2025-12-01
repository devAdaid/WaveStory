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
    [field: SerializeField]
    public LocalizedString SoulInOtherRoomMessage;

    private string wordId1;
    private string wordId2;

    private WordInputPresenter presenter;

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
        
        TextHelper.SetLocalizedTextEvent(wordText1, null, ref wordText1LocalizeEvent);
        TextHelper.SetLocalizedTextEvent(wordText2, null, ref wordText2LocalizeEvent);
    }

    public override void OnShow()
    {
        inventoryUI.Show();
    }

    public override void OnHide()
    {
        inventoryUI.Hide();
        AudioManager.I.PlaySfxOneShot("Click");
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

        if (string.IsNullOrEmpty(wordId1) == false && StaticDataHolder.I.TryGetWord(wordId1, out var wordData1))
        {  
            wordText1LocalizeEvent.StringReference = wordData1.DisplayText;
            wordText1LocalizeEvent.RefreshString();
        }
        else
        {
            wordText1LocalizeEvent.StringReference = new LocalizedString("Message", "Empty_Message");
            wordText1LocalizeEvent.RefreshString();
        }
        
        if (string.IsNullOrEmpty(wordId2) == false && StaticDataHolder.I.TryGetWord(wordId2, out var wordData2))
        {   
            wordText2LocalizeEvent.StringReference = wordData2.DisplayText;
            wordText2LocalizeEvent.RefreshString();
        }
        else
        {
            wordText2LocalizeEvent.StringReference = new LocalizedString("Message", "Empty_Message");
            wordText2LocalizeEvent.RefreshString();
        }
    }
}
