using RedBlueGames.Tools.TextTyper;
using System.Collections;
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
    [SerializeField]
    private RectTransform shakeTarget; // 흔들 대상 (dialogueWindow 또는 Canvas 전체)

    private DialogueCommandFactory commandFactory;
    private DialoguePlayer player;

    private bool isWatingChoice;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;
    private Coroutine waitCoroutine;

    protected override void InitializeInternal()
    {
        commandFactory = new DialogueCommandFactory();
        player = new DialoguePlayer(this, commandFactory, GM.I.Unlock);

        advanceButton.onClick.AddListener(OnDialogueInput);
        clueButton.onClick.AddListener(GM.I.UIHolder.ClueUI.Show);

        dialogueTextTyper.CharacterPrinted.AddListener(HandleCharacterPrinted);

        choiceRoot.SetActive(false);
        bgImage.gameObject.SetActive(false);
        HidePortrait();

        // 원본 위치 저장
        if (shakeTarget != null)
        {
            originalPosition = shakeTarget.localPosition;
        }
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
            OnDialogueInput();
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

    private void OnDialogueInput()
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

        if (waitCoroutine != null)
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

    public void ShowChoices(List<string> choices, System.Action<int> onSelected)
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
            return;
        }

        // 타이핑 중이면 먼저 스킵
        if (dialogueTextTyper.IsTyping)
        {
            dialogueTextTyper.Skip();
        }

        player.SkipToNextChoice();
    }

    public void ShakeScreen(float duration, float intensity)
    {
        if (shakeTarget == null)
        {
            Debug.LogWarning("[DialogueUI] Shake target is not assigned!");
            return;
        }

        // 이미 흔들리는 중이면 중단하고 새로 시작
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 랜덤 오프셋 생성
            float offsetX = Random.Range(-1f, 1f) * intensity;
            float offsetY = Random.Range(-1f, 1f) * intensity;

            shakeTarget.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복귀
        shakeTarget.localPosition = originalPosition;
        shakeCoroutine = null;
    }
    public void StopWait()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }
    public void WaitAndContinue(float duration)
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
        }
        waitCoroutine = StartCoroutine(WaitCoroutine(duration));
    }

    private IEnumerator WaitCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        waitCoroutine = null;
        player.ContinueDialogue();
    }
}
