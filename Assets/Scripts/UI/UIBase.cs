using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    public bool IsActive => root.activeSelf;

    protected void Start()
    {
        Initialize();
    }

    public abstract void Initialize();

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
