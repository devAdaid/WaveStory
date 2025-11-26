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
    [SerializeField]
    private Button clueButton;

    private DialogueCommandFactory commandFactory;
    private DialoguePlayer player;

    private bool isWatingChoice;

    protected override void InitializeInternal()
    {
        commandFactory = new DialogueCommandFactory();
        player = new DialoguePlayer(this, commandFactory, GM.I.Unlock);

        advanceButton.onClick.AddListener(AdvanceDialogue);
        clueButton.onClick.AddListener(GM.I.UIHolder.ClueUI.Show);

        dialogueTextTyper.CharacterPrinted.AddListener(HandleCharacterPrinted);

        choiceRoot.SetActive(false);
        bgImage.gameObject.SetActive(false);
        HidePortrait();
    }

    private void HandleCharacterPrinted(string printedCharacter)
    {
        if (printedCharacter == " " || printedCharacter == "\n")
        {
            return;
        }

        AudioManager.I.PlaySfx("Type");
    }

    private void Update()
    {
        if (!IsActive)
        {
            return;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            AdvanceDialogue();
        }

        // Ctrl 키를 누르고 있으면 스킵
#if UNITY_EDITOR
        if (player.IsActive && Input.GetKey(KeyCode.LeftControl))
        {
            SkipToNextChoice();
        }
#endif
        if (isWatingChoice && Input.GetKeyDown(KeyCode.C))
        {
            GM.I.UIHolder.ClueUI.Show();
        }
    }

    private void AdvanceDialogue()
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
        AudioManager.I.ReserveCurrentBgm();
        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(() => DoPlayDialogue(dialogue));
    }

    private void DoPlayDialogue(TextAsset dialogue)
    {
        ResetBeforePlay();
        player.LoadDialogue(dialogue);
        player.StartDialogue();
    }

    public void ForceEndDialogue()
    {

    }

    public void OnEndDialogue()
    {
        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(Hide);

        AudioManager.I.PlayReservedBgm();
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
        if (sprite)
        {
            bgImage.sprite = sprite;
            bgImage.gameObject.SetActive(true);
        }
        else
        {
            bgImage.gameObject.SetActive(false);
        }
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

    public void SkipToNextChoice()
    {
        if (isWatingChoice)
        {
            Debug.Log("[DialogueUI] Already at a choice");
            return;
        }

        // 타이핑 중이면 먼저 스킵
        if (dialogueTextTyper.IsTyping)
        {
            dialogueTextTyper.Skip();
        }

        player.SkipToNextChoice();
    }
}
