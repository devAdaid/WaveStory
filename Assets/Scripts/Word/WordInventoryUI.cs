using System;
using System.Collections.Generic;
using UnityEngine;

public class WordInventoryUI : UIBase, IView<WordInventoryPresenter>
{
    [SerializeField]
    private RectTransform buttonRoot;
    [SerializeField]
    private WordButton buttonTemplate;
    [SerializeField]
    private int buttonInitialCount;

    private WordInventoryPresenter presenter;
    private List<WordButton> buttonPool = new List<WordButton>();
    private Action<string> onClickWordButton;

    public void SetPresenter(WordInventoryPresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
        // buttonRoot에 buttonInitialCount만큼 추가하고 setactive false한다
        for (int i = 0; i < buttonInitialCount; i++)
        {
            WordButton button = Instantiate(buttonTemplate, buttonRoot);
            button.gameObject.SetActive(false);
            buttonPool.Add(button);
        }

        // 템플릿은 비활성화
        buttonTemplate.gameObject.SetActive(false);

        Apply(presenter.GetWordIds());
    }

    public void SetCallback(Action<string> wordButtonCallback)
    {
        onClickWordButton = wordButtonCallback;
    }

    public void Apply(List<string> wordIds)
    {
        // wordIds보다 부족한 개수만큼 button을 추가로 만든다
        int neededCount = wordIds.Count;
        while (buttonPool.Count < neededCount)
        {
            WordButton button = Instantiate(buttonTemplate, buttonRoot);
            button.gameObject.SetActive(false);
            buttonPool.Add(button);
        }

        // wordIds에 해당하는 button들을 Apply하고 활성화
        for (int i = 0; i < wordIds.Count; i++)
        {
            buttonPool[i].Apply(wordIds[i], OnClickButton);
            buttonPool[i].gameObject.SetActive(true);
        }

        // 나머지 버튼은 setactive false한다
        for (int i = wordIds.Count; i < buttonPool.Count; i++)
        {
            buttonPool[i].gameObject.SetActive(false);
        }
    }

    private void OnClickButton(string wordId)
    {
        onClickWordButton.Invoke(wordId);
    }
}