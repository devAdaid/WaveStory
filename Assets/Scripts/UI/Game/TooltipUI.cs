using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class TooltipUI : UIBase
{
    [Header("UI References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(10, -10);
    [SerializeField] private float padding = 10f;

    private InteractableBase currentTrigger;

    protected override void InitializeInternal()
    {
        Hide();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    public void ShowTooltip(LocalizedString text, InteractableBase trigger)
    {
        if (string.IsNullOrEmpty(text.GetLocalizedString())) return;

        this.Show();

        currentTrigger = trigger;
        tooltipText.text = text.GetLocalizedString();
        tooltipPanel.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        UpdateTooltipPosition();
    }

    public void HideTooltip(InteractableBase trigger)
    {
        // 현재 활성화된 툴팁이 요청한 트리거와 같을 때만 숨김
        if (currentTrigger == trigger)
        {
            tooltipPanel.SetActive(false);
            currentTrigger = null;

            this.Hide();
        }
    }

    private void UpdateTooltipPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        // 마우스 위치를 캔버스 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        // 기본 위치 설정 (마우스 오른쪽 아래)
        Vector2 tooltipPosition = localPoint + offset;

        // 캔버스 영역
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect != null)
        {
            Vector2 canvasSize = canvasRect.sizeDelta;

            // 툴팁이 화면을 벗어나지 않도록 조정
            float tooltipWidth = tooltipRect.sizeDelta.x;
            float tooltipHeight = tooltipRect.sizeDelta.y;

            // 오른쪽 경계 체크
            if (tooltipPosition.x + tooltipWidth / 2 > canvasSize.x / 2)
            {
                tooltipPosition.x = localPoint.x - tooltipWidth / 2 - offset.x - padding;
            }
            else
            {
                tooltipPosition.x += tooltipWidth / 2;
            }

            // 왼쪽 경계 체크
            if (tooltipPosition.x - tooltipWidth / 2 < -canvasSize.x / 2)
            {
                tooltipPosition.x = -canvasSize.x / 2 + tooltipWidth / 2 + padding;
            }

            // 아래쪽 경계 체크
            if (tooltipPosition.y - tooltipHeight / 2 < -canvasSize.y / 2)
            {
                tooltipPosition.y = localPoint.y + tooltipHeight / 2 - offset.y + padding;
            }
            else
            {
                tooltipPosition.y -= tooltipHeight / 2;
            }

            // 위쪽 경계 체크
            if (tooltipPosition.y + tooltipHeight / 2 > canvasSize.y / 2)
            {
                tooltipPosition.y = canvasSize.y / 2 - tooltipHeight / 2 - padding;
            }
        }

        tooltipRect.localPosition = tooltipPosition;
    }
}
