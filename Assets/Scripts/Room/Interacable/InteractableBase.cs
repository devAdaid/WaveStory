using System.Collections.Generic;
using UnityEngine;
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
    private Button button;

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
        OnInteract();
    }

    public abstract void OnInteract();

    public void Apply(bool isSoulMode, UnlockState context)
    {
        var isActive = true;
        switch (interactableType)
        {
            case InteractableType.Always:
                break;
            case InteractableType.OnlyRealMode:
                button.interactable = !isSoulMode;
                break;
            case InteractableType.OnlySoulMode:
                isActive &= isSoulMode;
                break;
        }

        isActive &= activeCondition.IsSatisfiedBy(context);
        gameObject.SetActive(isActive);

        ApplyUnlock(context);
    }

    protected virtual void ApplyUnlock(UnlockState context) { }
}
