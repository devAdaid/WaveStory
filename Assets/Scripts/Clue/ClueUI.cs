using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private Button toListButton;

    private ClueData currentClueData;
    private CluePresenter presenter;

    private LocalizeStringEvent titleStringEvent;
    private LocalizeStringEvent descriptionStringEvent;

    public void SetPresenter(CluePresenter presenter)
    {
        this.presenter = presenter;
    }

    protected override void InitializeInternal()
    {
        toListButton.onClick.AddListener(OpenList);

        TextHelper.SetLocalizedText(clueTitleText, null, ref titleStringEvent);
        TextHelper.SetLocalizedText(descriptionText, null, ref descriptionStringEvent);

        UpdateUI();
    }

    public override void OnShow()
    {
        AudioManager.I.PlaySfxOneShot("Paper");
        StartCoroutine(ShowAnim());
    }

    private IEnumerator ShowAnim()
    {
        GM.I.UIHolder.InputBlocker.SetActive(true);
        contentObject.SetActive(false);

        animator.SetTrigger("Show");

        yield return StartCoroutine(WaitForAnimationToStart("Idle"));

        contentObject.SetActive(true);
        GM.I.UIHolder.InputBlocker.SetActive(false);
    }

    public override IEnumerator BeforeHide()
    {
        AudioManager.I.PlaySfxOneShot("Paper");
        GM.I.UIHolder.InputBlocker.SetActive(true);
        contentObject.SetActive(false);

        animator.SetTrigger("Hide");

        yield return null;

        yield return StartCoroutine(WaitForAnimationToStart("Hidden"));

        GM.I.UIHolder.InputBlocker.SetActive(false);
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

        foreach (var wordData in clueData.UnlockWords)
        {
            presenter.AddWord(wordData.Id);
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
}
