using System;
using UnityEngine;
using UnityEngine.UI;

public class EndingSoul : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private WaveHand waveHand;

    private RectTransform rectTransform;
    private Image image;
    private bool isAnimating = false;
    private bool successReceived = false;
    private Canvas rootCanvas;
    private RectTransform canvasRect;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        canvasRect = rootCanvas.GetComponent<RectTransform>();
        waveHand.OnSuccess.AddListener(OnWaveHandSuccess);
    }

    public async Awaitable StartElement(float targetX)
    {
        successReceived = false;

        await Appear(targetX);
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    private void OnWaveHandSuccess()
    {
        successReceived = true;
    }

    public async Awaitable WaitForSuccessAsync()
    {
        while (!successReceived)
        {
            await Awaitable.NextFrameAsync();
        }
    }

    /// <summary>
    /// 화면 오른쪽 끝에서 targetX 위치까지 슬라이드 인 애니메이션
    /// </summary>
    /// <param name="targetX">화면 안쪽 목표 X 좌표 (anchoredPosition 기준)</param>
    public async Awaitable Appear(float targetX)
    {
        if (isAnimating) return;

        float offScreenX = CalculateOffScreenRightX();
        await AnimateAsync(offScreenX, targetX, 0f, 1f);
    }

    /// <summary>
    /// 현재 위치에서 화면 오른쪽 끝으로 슬라이드 아웃 애니메이션
    /// </summary>
    public async Awaitable DisappearRight()
    {
        if (isAnimating) return;

        float currentX = rectTransform.anchoredPosition.x;
        float offScreenX = CalculateOffScreenRightX();
        await AnimateAsync(currentX, offScreenX, 1f, 0f);
    }

    /// <summary>
    /// 현재 위치에서 화면 왼쪽 끝으로 슬라이드 아웃 애니메이션
    /// </summary>
    public async Awaitable DisappearLeft()
    {
        if (isAnimating) return;

        float currentX = rectTransform.anchoredPosition.x;
        float offScreenX = CalculateOffScreenLeftX();
        await AnimateAsync(currentX, offScreenX, 1f, 0f);
    }

    private async Awaitable AnimateAsync(float startX, float endX, float startAlpha, float endAlpha)
    {
        isAnimating = true;

        // 시작 상태 설정
        Vector2 pos = rectTransform.anchoredPosition;
        pos.x = startX;
        rectTransform.anchoredPosition = pos;

        Color color = image.color;
        color.a = startAlpha;
        image.color = color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 위치 보간
            pos.x = Mathf.Lerp(startX, endX, smoothT);
            rectTransform.anchoredPosition = pos;

            // 알파 보간
            color.a = Mathf.Lerp(startAlpha, endAlpha, smoothT);
            image.color = color;

            await Awaitable.NextFrameAsync();
        }

        // 최종 상태 보장
        pos.x = endX;
        rectTransform.anchoredPosition = pos;

        color.a = endAlpha;
        image.color = color;

        isAnimating = false;
    }

    /// <summary>
    /// 이미지가 완전히 화면 오른쪽 밖으로 나가는 X 좌표 계산 (앵커/피벗 무관)
    /// </summary>
    private float CalculateOffScreenRightX()
    {
        // Canvas의 월드 좌표 모서리
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        float canvasRightWorld = canvasCorners[2].x;

        // 이미지의 월드 좌표 모서리
        Vector3[] myCorners = new Vector3[4];
        rectTransform.GetWorldCorners(myCorners);
        float myLeftWorld = myCorners[0].x;

        // 현재 위치에서 얼마나 더 이동해야 화면 밖인지 계산
        // 이미지의 왼쪽 끝이 화면 오른쪽 끝에 딱 붙는 위치
        float currentX = rectTransform.anchoredPosition.x;
        float distanceToEdge = canvasRightWorld - myLeftWorld;

        // Canvas 스케일 고려
        float canvasScale = canvasRect.lossyScale.x;
        float offsetNeeded = distanceToEdge / canvasScale;

        return currentX + offsetNeeded;
    }

    /// <summary>
    /// 이미지가 완전히 화면 왼쪽 밖으로 나가는 X 좌표 계산 (앵커/피벗 무관)
    /// </summary>
    private float CalculateOffScreenLeftX()
    {
        // Canvas의 월드 좌표 모서리
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        float canvasLeftWorld = canvasCorners[0].x;

        // 이미지의 월드 좌표 모서리
        Vector3[] myCorners = new Vector3[4];
        rectTransform.GetWorldCorners(myCorners);
        float myRightWorld = myCorners[2].x;

        // 현재 위치에서 얼마나 더 이동해야 화면 밖인지 계산
        // 이미지의 오른쪽 끝이 화면 왼쪽 끝에 딱 붙는 위치
        float currentX = rectTransform.anchoredPosition.x;
        float distanceToEdge = canvasLeftWorld - myRightWorld;

        // Canvas 스케일 고려
        float canvasScale = canvasRect.lossyScale.x;
        float offsetNeeded = distanceToEdge / canvasScale;

        return currentX + offsetNeeded;
    }
}
