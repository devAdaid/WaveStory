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

    [Header("Success Condition")]
    [SerializeField] private int requiredCycles = 3;

    [Header("Events")]
    public UnityEvent OnSuccess;

    private RectTransform rectTransform;
    private float currentAngle;
    private bool isDragging;
    private Vector2 lastDragPosition;

    private bool wasAtMin;
    private bool wasAtMax;
    private int cycleCount;
    private bool successTriggered;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        currentAngle = rectTransform.localEulerAngles.z;
        if (currentAngle > 180f) currentAngle -= 360f;
    }

    private void Update()
    {
        if (successTriggered) return;

        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
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
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (successTriggered) return;

        isDragging = true;
        lastDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (successTriggered || !isDragging) return;

        float deltaX = eventData.position.x - lastDragPosition.x;
        lastDragPosition = eventData.position;

        float delta = deltaX * rotationSensitivity;
        ApplyRotation(delta);
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

        CheckCycle(previousAngle);
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
        //currentAngle = 0f;
        //rectTransform.localEulerAngles = Vector3.zero;
    }

    public float GetCurrentAngle() => currentAngle;
    public int GetCycleCount() => cycleCount;
    public bool IsSuccess() => successTriggered;
}
