using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WaveHand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Rotation Settings")]
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float rotationSensitivity = 0.5f;
    [SerializeField] private float keyboardRotationSpeed = 100f;

    [Header("Position Settings")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;

    [Header("Return Settings")]
    [SerializeField] private float defaultAngle = 0f;
    [SerializeField] private float returnSpeed = 90f;

    [Header("Success Condition")]
    [SerializeField] private int requiredCycles = 3;

    [Header("Events")]
    public UnityEvent OnSuccess;

    private RectTransform rectTransform;
    private float currentAngle;
    private bool isDragging;
    private float initialX;

    // 각도 기반 드래그를 위한 변수
    private float dragStartAngle;
    private float accumulatedAngle;
    private float dragStartRotation;

    private bool wasAtMin;
    private bool wasAtMax;
    private int cycleCount;
    private bool successTriggered;
    private bool isInputEnabled = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        currentAngle = rectTransform.localEulerAngles.z;
        if (currentAngle > 180f) currentAngle -= 360f;
        initialX = rectTransform.anchoredPosition.x;
    }

    private void Update()
    {
        bool hasInput = false;

        // 입력 처리는 isInputEnabled가 true이고 successTriggered가 false일 때만
        if (isInputEnabled && !successTriggered)
        {
            hasInput = HandleKeyboardInput();
        }

        // 복귀는 입력 상태와 무관하게 작동 (드래그 중이 아니고 입력이 없을 때)
        if (!isDragging && !hasInput && !successTriggered)
        {
            ReturnToDefault();
        }
    }

    private bool HandleKeyboardInput()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            input = 1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            input = -1f;
        }

        if (input != 0f)
        {
            float delta = input * keyboardRotationSpeed * Time.deltaTime;
            ApplyRotation(delta);
            return true;
        }

        return false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInputEnabled || successTriggered) return;

        isDragging = true;

        // 중심점에서 마우스 위치까지의 각도 계산
        Vector2 center = rectTransform.position;
        Vector2 direction = eventData.position - center;
        dragStartAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        accumulatedAngle = 0f;
        dragStartRotation = currentAngle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInputEnabled || successTriggered || !isDragging) return;

        // 현재 마우스 위치의 각도 계산
        Vector2 center = rectTransform.position;
        Vector2 direction = eventData.position - center;
        float currentMouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 각도 변화량 계산 (180도 경계 안전 처리)
        float angleDelta = Mathf.DeltaAngle(dragStartAngle + accumulatedAngle, currentMouseAngle);
        accumulatedAngle += angleDelta;

        // 회전 적용
        float previousAngle = currentAngle;
        float targetAngle = dragStartRotation + (accumulatedAngle * rotationSensitivity);
        currentAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
        rectTransform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
        UpdatePosition();

        CheckCycle(previousAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void ApplyRotation(float delta)
    {
        float previousAngle = currentAngle;
        currentAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

        rectTransform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
        UpdatePosition();

        CheckCycle(previousAngle);
    }

    private void UpdatePosition()
    {
        float t = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
        float targetX = Mathf.Lerp(minX, maxX, t);

        Vector2 pos = rectTransform.anchoredPosition;
        pos.x = initialX - targetX;
        rectTransform.anchoredPosition = pos;
    }

    private void ReturnToDefault()
    {
        if (Mathf.Approximately(currentAngle, defaultAngle)) return;

        currentAngle = Mathf.MoveTowards(currentAngle, defaultAngle, returnSpeed * Time.deltaTime);

        rectTransform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
        UpdatePosition();
    }

    private void CheckCycle(float previousAngle)
    {
        bool atMin = Mathf.Approximately(currentAngle, minAngle);
        bool atMax = Mathf.Approximately(currentAngle, maxAngle);

        if (atMin && !wasAtMin)
        {
            wasAtMin = true;
            if (wasAtMax)
            {
                wasAtMax = false;
                IncrementCycle();
            }
        }
        else if (atMax && !wasAtMax)
        {
            wasAtMax = true;
            if (wasAtMin)
            {
                wasAtMin = false;
                IncrementCycle();
            }
        }
    }

    private void IncrementCycle()
    {
        cycleCount++;

        AudioManager.I.PlaySfxOneShot("Click");

        if (cycleCount >= requiredCycles && !successTriggered)
        {
            successTriggered = true;
            OnSuccess?.Invoke();
        }
    }

    public void ResetState()
    {
        cycleCount = 0;
        wasAtMin = false;
        wasAtMax = false;
        successTriggered = false;
        isInputEnabled = false;
        //currentAngle = 0f;
        //rectTransform.localEulerAngles = Vector3.zero;
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
    }

    public float GetCurrentAngle() => currentAngle;
    public int GetCycleCount() => cycleCount;
    public bool IsSuccess() => successTriggered;
}
