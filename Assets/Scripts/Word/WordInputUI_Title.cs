using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordInputUI_Title : UIBase
{
    [SerializeField]
    private TMP_Text wordText1;

    [SerializeField]
    private TMP_Text wordText2;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private WordInventoryUI inventoryUI;

    private string wordId1;
    private string wordId2;

    private static string EMPTY_TEXT = "_________";

    protected override void InitializeInternal()
    {
        confirmButton.onClick.AddListener(OnClick);
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

    private void OnClick()
    {
        TitleUI.I.OnInput(wordId1, wordId2);
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

        wordText1.text = string.IsNullOrEmpty(wordId1) ? EMPTY_TEXT : StaticDataHolder.I.TryGetWord(wordId1, out var wordData1) ? wordData1.DisplayText : EMPTY_TEXT;
        wordText2.text = string.IsNullOrEmpty(wordId2) ? EMPTY_TEXT : StaticDataHolder.I.TryGetWord(wordId2, out var wordData2) ? wordData2.DisplayText : EMPTY_TEXT;
    }
}
