using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ClueUI : UIBase, IView<CluePresenter>
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject contentObject;

    [SerializeField]
    private GameObject listObject;
    [SerializeField]
    private List<ClueButtonItem> listClueButtons;

    [SerializeField]
    private GameObject clueObject;
    [SerializeField]
    private TMP_Text clueTitleText;
    [SerializeField]
    private TMP_Text descriptionText;
    [SerializeField]
    private TMP_Text descriptionText2;
    [SerializeField]
    private Button toListButton;

    [SerializeField] private LocalizedString newWordAddedMessage;

    private ClueData currentClueData;
    private CluePresenter presenter;

    private LocalizeStringEvent titleStringEvent;
    private LocalizeStringEvent descriptionStringEvent;
    private bool needsOverflowCheck = false;
    private bool isShowingAnim = false;

    private string newWordAlarmMessage = string.Empty;

    public void SetPresenter(CluePresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
        toListButton.onClick.AddListener(OpenList);

        TextHelper.SetLocalizedTextEvent(clueTitleText, null, ref titleStringEvent);
        TextHelper.SetLocalizedTextEvent(descriptionText, null, ref descriptionStringEvent);

        if (descriptionStringEvent != null)
        {
            descriptionStringEvent.OnUpdateString.AddListener(OnDescriptionTextChanged);
        }

        UpdateUI();
    }

    private void OnDescriptionTextChanged(string newText)
    {
        needsOverflowCheck = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && IsActive && !isShowingAnim)
        {
            Hide();
        }
    }

    private void LateUpdate()
    {
        if (needsOverflowCheck)
        {
            needsOverflowCheck = false;
            ApplyTextOverflow();
        }
    }

    public override void OnShow()
    {
        AudioManager.I.PlaySfxOneShot("Paper");
        StartCoroutine(ShowAnim());
    }

    private IEnumerator ShowAnim()
    {
        isShowingAnim = true;
        GM.I.UIHolder.InputBlocker.SetActive(true);
        contentObject.SetActive(false);

        animator.SetTrigger("Show");

        yield return StartCoroutine(WaitForAnimationToStart("Idle"));

        contentObject.SetActive(true);
        GM.I.UIHolder.InputBlocker.SetActive(false);
        isShowingAnim = false;
    }

    public override IEnumerator BeforeHide()
    {
        AudioManager.I.PlaySfxOneShot("PaperClose");
        GM.I.UIHolder.InputBlocker.SetActive(true);
        contentObject.SetActive(false);

        animator.SetTrigger("Hide");

        yield return null;

        yield return StartCoroutine(WaitForAnimationToStart("Hidden"));

        GM.I.UIHolder.InputBlocker.SetActive(false);

        if (!string.IsNullOrEmpty(newWordAlarmMessage))
        {
            GM.I.UIHolder.AlarmUI.ShowAlarm(newWordAlarmMessage);
            AudioManager.I.PlaySfxOneShot("NewWord");
        }
    }

    public IEnumerator WaitForAnimationToStart(string stateName, int layer = 0)
    {
        // 현재 프레임이 끝날 때까지 대기
        yield return null;

        // 애니메이션 상태가 해당 stateName이 될 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            yield return null;
        }
    }

    public IEnumerator WaitForAnimationToEnd(string stateName, int layer = 0)
    {
        // 애니메이션이 시작될 때까지 대기
        yield return WaitForAnimationToStart(stateName, layer);

        // 애니메이션이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName) &&
               animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }

    public void OpenClue(ClueData clueData)
    {
        currentClueData = clueData;

        var addedWords = new List<WordData>();

        foreach (var wordData in clueData.UnlockWords)
        {
            var isWordAdded = presenter.AddWord(wordData.Id, clueData.Floor);

            if (isWordAdded)
            {
                addedWords.Add(wordData);
            }
        }

        newWordAlarmMessage = string.Empty;
        if (addedWords.Count > 0)
        {
            var wordStr = string.Join(", ", addedWords.Select(x => $"'{x.DisplayText.GetLocalizedStringAsync().WaitForCompletion()}'"));
            newWordAlarmMessage = $"{newWordAddedMessage.GetLocalizedStringAsync().WaitForCompletion()}" + wordStr;
        }

        presenter.UnlockClue(clueData.Id);

        UpdateUI();

        Show();
    }

    public void MoveToClue(ClueData clueData)
    {
        currentClueData = clueData;

        UpdateUI();

        AudioManager.I.PlaySfxOneShot("Paper");
    }

    private void OpenList()
    {
        currentClueData = null;
        UpdateUI();
        AudioManager.I.PlaySfxOneShot("Paper");
    }

    public void UpdateUI()
    {
        if (currentClueData)
        {
            clueObject.SetActive(true);
            listObject.SetActive(false);

            titleStringEvent.StringReference = currentClueData.Title;
            titleStringEvent.RefreshString();

            descriptionStringEvent.StringReference = currentClueData.Text;
            descriptionStringEvent.RefreshString();
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

    private void ApplyTextOverflow()
    {
        string fullText = descriptionText.text;

        if (string.IsNullOrEmpty(fullText))
        {
            descriptionText2.text = "";
            descriptionText2.gameObject.SetActive(false);
            return;
        }

        // 강제 메시 업데이트
        Canvas.ForceUpdateCanvases();
        descriptionText.ForceMeshUpdate(true, true);

        TMP_TextInfo textInfo = descriptionText.textInfo;

        // 마지막으로 보이는 문자 찾기
        int lastVisibleCharIndex = -1;

        if (textInfo.characterCount > 0)
        {
            for (int i = textInfo.characterCount - 1; i >= 0; i--)
            {
                if (textInfo.characterInfo[i].isVisible)
                {
                    lastVisibleCharIndex = i;
                    break;
                }
            }
        }

        // 오버플로우 발생 여부 확인
        if (lastVisibleCharIndex >= 0 && lastVisibleCharIndex < fullText.Length - 1)
        {
            // 오버플로우 발생
            string firstPart = fullText.Substring(0, lastVisibleCharIndex + 1);
            string secondPart = fullText.Substring(lastVisibleCharIndex + 1);

            descriptionText.text = firstPart;
            descriptionText2.text = secondPart;
            descriptionText2.gameObject.SetActive(true);
        }
        else
        {
            // 오버플로우 없음
            descriptionText2.text = "";
            descriptionText2.gameObject.SetActive(false);
        }
    }
}