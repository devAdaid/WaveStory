using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class InteractableBase : MonoBehaviour
{

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
}
