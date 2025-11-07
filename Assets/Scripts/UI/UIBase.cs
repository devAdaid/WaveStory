using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    public bool IsActive => root.activeSelf;

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
