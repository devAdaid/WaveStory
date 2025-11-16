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
    private Button button;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        OnInteract();
    }

    public abstract void OnInteract();

    public void OnSoulModeChange(bool isSoulMode)
    {
        switch (interactableType)
        {
            case InteractableType.Always:
                return;
            case InteractableType.OnlyRealMode:
                button.interactable = !isSoulMode;
                break;
            case InteractableType.OnlySoulMode:
                gameObject.SetActive(isSoulMode);
                break;
        }
    }
}
