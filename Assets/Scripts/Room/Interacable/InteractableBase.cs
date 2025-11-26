using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

public enum InteractableType
{
    Always,
    OnlyRealMode,
    OnlySoulMode,
}

[RequireComponent(typeof(Button))]
public abstract class InteractableBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField]
    private UnlockCondition activeCondition;

    [SerializeField]
    private UnlockCondition interactableCondition;

    [SerializeField]
    private Button button;

    protected abstract InteractableType interactableType { get; }
    protected abstract string GetTooltipText();
    protected virtual LocalizedString notInteractableMessage => new LocalizedString("Message", "Invalid");

    private bool isInteractable = true;

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


    public void OnPointerEnter(PointerEventData eventData)
    {
        GM.I.UIHolder.TooltilUI.ShowTooltip(GetTooltipText(), this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.I.UIHolder.TooltilUI.HideTooltip(this);
    }

    public void OnDisable()
    {
        if (GM.IsInitialized)
        {
            GM.I.UIHolder.TooltilUI.HideTooltip(this);
        }
    }
}
