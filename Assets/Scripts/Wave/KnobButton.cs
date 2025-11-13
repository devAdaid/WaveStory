using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnobButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform knobRectTransform;

    [SerializeField]
    private float maxRotationAngle = 120f;

    [SerializeField]
    private bool invertDrag = false;

    [SerializeField]
    private GameObject arrow;

    private int minStep;
    private int maxStep;
    private int step;

    private bool isDragging = false;
    private float startAngle;
    private int startStep;
    private float accumulatedAngle;

    public int CurrentStep => step;

    private Action<int> onStepChanged;

    private bool isChangeBlock;

    private bool soundFlag;

    public void Initialize(int minStep, int maxStep, int initialStep, Action<int> onStepChanged)
    {
        this.minStep = minStep;
        this.maxStep = maxStep;
        this.step = Mathf.Clamp(initialStep, minStep, maxStep);
        this.onStepChanged = onStepChanged;

        UpdateKnobRotation();
    }

    public void SetChangeBlock(bool isChangeBlock)
    {
        this.isChangeBlock = isChangeBlock;
        isDragging = false;
    }

    public void SetArrowActive(bool active)
    {
        if (arrow)
        {
            arrow.SetActive(active);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isChangeBlock) return;

        isDragging = true;

        Vector2 localPointerPosition = GetLocalPointerPosition(eventData);
        Vector2 knobCenter = knobRectTransform.localPosition;
        Vector2 direction = localPointerPosition - knobCenter;

        startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        startStep = step;
        accumulatedAngle = 0f;

        SetArrowActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        if (isChangeBlock) return;

        Vector2 localPointerPosition = GetLocalPointerPosition(eventData);
        Vector2 knobCenter = knobRectTransform.localPosition;
        Vector2 direction = localPointerPosition - knobCenter;

        float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 누적 각도에 더하기 (180도 점프 방지)
        float angleDelta = Mathf.DeltaAngle(startAngle + accumulatedAngle, currentAngle);
        accumulatedAngle += angleDelta;

        // 회전 방향이 반대가 되었으므로 각도도 반대로
        float finalAngle = invertDrag ? accumulatedAngle : -accumulatedAngle;

        // 각도를 step으로 변환
        float totalAngleRange = maxRotationAngle * 2f;
        int totalStepRange = maxStep - minStep;

        float stepChange = (finalAngle / totalAngleRange) * totalStepRange;
        int newStep = Mathf.RoundToInt(startStep + stepChange);
        newStep = Mathf.Clamp(newStep, minStep, maxStep);

        if (newStep != step)
        {
            step = newStep;
            UpdateKnobRotation();
            onStepChanged?.Invoke(step);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void UpdateKnobRotation()
    {
        if (knobRectTransform == null) return;

        float normalizedValue = 0f;
        if (maxStep != minStep)
        {
            normalizedValue = (float)(step - minStep) / (maxStep - minStep);
        }

        // minStep일 때 +120도, maxStep일 때 -120도
        float targetAngle = Mathf.Lerp(maxRotationAngle, -maxRotationAngle, normalizedValue);

        knobRectTransform.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    private Vector2 GetLocalPointerPosition(PointerEventData eventData)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            knobRectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPosition
        );
        return localPosition;
    }

    public void SetStep(int newStep, bool withSound = false)
    {
        newStep = Mathf.Clamp(newStep, minStep, maxStep);

        if (step == newStep) return;

        step = newStep;

        UpdateKnobRotation();


        if (withSound)
        {
            AudioManager.I.PlaySfxOneShot(soundFlag ? "Knob_1" : "Knob_2");
            soundFlag = !soundFlag;
        }
    }

    public void AddStep(int delta)
    {
        SetStep(step + delta);
        onStepChanged?.Invoke(step);
    }
}