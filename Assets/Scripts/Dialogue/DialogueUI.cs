using RedBlueGames.Tools.TextTyper;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    [SerializeField]
    private Button advanceButton;
    [SerializeField]
    private TextTyper dialogueTextTyper;
    [SerializeField]
    private TextMeshProUGUI speakerNameText;
    [SerializeField]
    private Image portraitImage;
    [SerializeField]
    private GameObject choiceRoot;
    [SerializeField]
    private List<DialogueChoiceButton> choiceButtons;
    [SerializeField]
    private GameObject dialogueWindow;
    [SerializeField]
    private Image bgImage;
    [SerializeField]
    private Image fullscreenImage;

    private DialogueCommandFactory commandFactory;
    private DialoguePlayer player;

    private bool isWatingChoice;

    protected override void InitializeInternal()
    {
        commandFactory = new DialogueCommandFactory();
        player = new DialoguePlayer(this, commandFactory, GM.I.Unlock);

        advanceButton.onClick.AddListener(OnAdvance);

        choiceRoot.SetActive(false);
        HidePortrait();
    }

    private void Update()
    {
        if (IsActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            OnAdvance();
        }
    }

    private void OnAdvance()
    {
        if (dialogueTextTyper.IsTyping)
        {
            dialogueTextTyper.Skip();
            return;
        }

        if (isWatingChoice)
        {
            return;
        }

        player.OnPlayerAdvance();
    }

    public void PlayDialogue(TextAsset dialogue)
    {
        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(() => DoPlayDialogue(dialogue));
    }

    private void DoPlayDialogue(TextAsset dialogue)
    {
        ResetBeforePlay();
        player.LoadDialogue(dialogue);
        player.StartDialogue();
    }

    public void OnEndDialogue()
    {
        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(Hide);
    }

    private void ResetBeforePlay()
    {
        HidePortrait();
        SetSpeakerName(string.Empty);
    }

    public void ShowText(string text)
    {
        dialogueTextTyper.TypeText(text);
    }

    public void SetSpeakerName(string name)
    {
        speakerNameText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(name));
        speakerNameText.text = name;
    }

    public void ShowChoices(List<string> choices, Action<int> onSelected)
    {
        choiceRoot.SetActive(true);

        // 모든 선택지 버튼 비활성화
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        // 필요한 만큼만 활성화하고 설정
        for (int i = 0; i < choices.Count && i < choiceButtons.Count; i++)
        {
            int choiceIndex = i; // 클로저 문제 방지
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].Apply(choices[i], () =>
            {
                onSelected(choiceIndex);
                choiceRoot.SetActive(false);
            });
        }

        isWatingChoice = true;
    }

    public void OnChoiceSelected()
    {
        isWatingChoice = false;
    }

    public void ShowFullscreenImage(Sprite sprite)
    {
        fullscreenImage.sprite = sprite;
        fullscreenImage.gameObject.SetActive(true);
    }

    public void SetBg(Sprite sprite)
    {
        bgImage.sprite = sprite;
    }

    public void SetPortrait(Sprite sprite)
    {
        portraitImage.sprite = sprite;
        portraitImage.gameObject.SetActive(true);
    }

    public void HidePortrait()
    {
        portraitImage.gameObject.SetActive(false);
    }
}
