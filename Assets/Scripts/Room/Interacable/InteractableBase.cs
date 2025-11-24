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
    private InteractableType interactableType;

    [SerializeField]
    private UnlockCondition activeCondition;

    [SerializeField]
    private UnlockCondition interactableCondition;

    [SerializeField]
    private LocalizedString notInteractableMessage;

    [SerializeField]
    private Button button;

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
        var interactable = true;
        var isActive = true;
        switch (interactableType)
        {
            case InteractableType.Always:
                break;
            case InteractableType.OnlyRealMode:
                interactable &= !isSoulMode;
                break;
            case InteractableType.OnlySoulMode:
                isActive &= isSoulMode;
                break;
        }

        isActive &= activeCondition.IsSatisfiedBy(context);
        gameObject.SetActive(isActive);

        interactable &= interactableCondition.IsSatisfiedBy(context);
        isInteractable = interactable;

        ApplyUnlock(context);
    }

    protected virtual void ApplyUnlock(UnlockState context) { }
}
