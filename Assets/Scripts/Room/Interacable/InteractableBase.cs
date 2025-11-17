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

    public void Apply(bool isSoulMode, Dictionary<string, SoulState> soulStates, HashSet<string> flags)
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

        isActive &= activeCondition.SatisfyCondition(soulStates, flags);
        gameObject.SetActive(isActive);

        ApplyUnlock(soulStates, flags);
    }

    protected virtual void ApplyUnlock(Dictionary<string, SoulState> soulStates, HashSet<string> flags) { }
}
