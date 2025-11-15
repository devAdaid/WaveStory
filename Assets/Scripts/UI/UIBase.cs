using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    [SerializeField]
    private List<Button> closeButtons;

    public bool IsActive => root.activeSelf;

    protected bool isInitialized;

    public void Initialize()
    {
        if (isInitialized) return;

        InitializeInternal();

        isInitialized = true;
    }

    protected abstract void InitializeInternal();

    private void Awake()
    {
        foreach (var button in closeButtons)
        {
            button.onClick.AddListener(Hide);
        }
    }

    public void Show()
    {
        root.SetActive(true);
        OnShow();
    }

    public virtual void OnShow() { }

    public void Hide()
    {
        root.SetActive(false);
        OnHide();
    }

    public virtual void OnHide() { }
}
