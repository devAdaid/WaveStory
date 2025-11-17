using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueUI : UIBase, IView<CluePresenter>
{
    [SerializeField]
    private GameObject listObject;
    [SerializeField]
    private List<ClueButtonItem> listClueButtons;

    [SerializeField]
    private GameObject clueObject;
    [SerializeField]
    private TMP_Text clueTitleText;
    [SerializeField]
    private TMP_Text clueText;
    [SerializeField]
    private Button toListButton;

    private ClueData currentClueData;
    private CluePresenter presenter;

    public void SetPresenter(CluePresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
        toListButton.onClick.AddListener(OpenList);

        UpdateUI();
    }

    public void OpenClue(ClueData clueData)
    {
        currentClueData = clueData;

        foreach (var wordData in clueData.UnlockWords)
        {
            presenter.AddWord(wordData.Id);
        }
        presenter.UnlockClue(clueData.Id);

        UpdateUI();

        Show();
    }

    private void OpenList()
    {
        currentClueData = null;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (currentClueData)
        {
            clueObject.SetActive(true);
            listObject.SetActive(false);
            clueTitleText.text = currentClueData.Title;
            clueText.text = currentClueData.Text;
        }
        else
        {
            clueObject.SetActive(false);
            listObject.SetActive(true);

            var clueDataList = StaticDataHolder.I.ClueDataList;
            for (var i = 0; i < clueDataList.Count; i++)
            {
                var clueData = clueDataList[i];
                listClueButtons[i].Apply(clueData, presenter.IsUnlocked(clueData.Id));
                listClueButtons[i].gameObject.SetActive(true);
            }

            for (var i = clueDataList.Count; i < listClueButtons.Count; i++)
            {
                listClueButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
