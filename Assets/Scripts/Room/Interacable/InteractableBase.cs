using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public enum InteractableType
{
    Always,
    OnlyRealMode,
    OnlySoulMode,
}

[RequireComponent(typeof(Button))]
public abstract class InteractableBase : MonoBehaviour
{

    [SerializeField]
    private UnlockCondition activeCondition;

    [SerializeField]
    private UnlockCondition interactableCondition;

    [SerializeField]
    private Button button;

    protected abstract InteractableType interactableType { get; }
    protected virtual LocalizedString notInteractableMessage => new LocalizedString("Message", "Invalid");

    private bool isInteractable;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
        Initialize();
    }

    public virtual void Initialize() { }

    private void OnClick()
    {
        if (isInteractable)
        {
            OnInteract();
        }
        else
        {
            GM.I.UIHolder.AlarmUI.ShowAlarm(notInteractableMessage.GetLocalizedStringAsync().WaitForCompletion());
        }
    }

    public abstract void OnInteract();

    public void Apply(bool isSoulMode, UnlockState context)
    {
        ApplyUnlock(context);

        gameObject.SetActive(IsActive(isSoulMode, context));
        isInteractable = IsInteractable(isSoulMode, context);
    }

    protected virtual bool IsActive(bool isSoulMode, UnlockState context)
    {
        if (interactableType == InteractableType.OnlySoulMode && !isSoulMode)
        {
            return false;
        }

        return activeCondition.IsSatisfiedBy(context);
    }

    protected virtual bool IsInteractable(bool isSoulMode, UnlockState context)
    {
        if (interactableType == InteractableType.OnlyRealMode && isSoulMode)
        {
            return false;
        }

        return interactableCondition.IsSatisfiedBy(context);
    }


    protected virtual void ApplyUnlock(UnlockState context) { }
}
