using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ClueButtonItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private Button button;

    private ClueData clueData;
    private bool isUnlocked;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Apply(ClueData clueData, bool isUnlocked)
    {
        // 이전 구독 해제
        if (this.clueData != null && this.clueData.Title != null)
        {
            this.clueData.Title.StringChanged -= OnStringChanged;
        }

        this.clueData = clueData;
        this.isUnlocked = isUnlocked;

        if (isUnlocked)
        {
            // LocalizedString의 변경 이벤트 구독
            clueData.Title.StringChanged += OnStringChanged;

            // 초기 값 설정 (비동기로 가져오기)
            var operation = clueData.Title.GetLocalizedStringAsync();
            operation.Completed += (op) =>
            {
                if (this != null && nameText != null)
                {
                    nameText.text = op.Result;
                }
            };
        }
        else
        {
            nameText.text = "???";
        }
    }

    private void OnStringChanged(string value)
    {
        if (nameText != null)
        {
            nameText.text = value;
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 구독 해제
        if (clueData != null && clueData.Title != null)
        {
            clueData.Title.StringChanged -= OnStringChanged;
        }
    }

    private void OnClick()
    {
        if (isUnlocked)
        {
            GM.I.UIHolder.ClueUI.MoveToClue(clueData);
        }
    }
}