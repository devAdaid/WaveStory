using System.Collections.Generic;
using UnityEngine;

public class PopupHandler : MonoSingleton<PopupHandler>, IMonoSingleton
{
    private Stack<UIBase> popupStack = new();

    public void Initialize()
    {
    }

    public void AddPopup(UIBase ui)
    {
        popupStack.Push(ui);
    }

    public void RemoveTop()
    {
        if (!IsAnyPopup())
        {
            return;
        }

        popupStack.Pop();
    }

    public bool IsOpened(UIBase ui)
    {
        return popupStack.Contains(ui);
    }

    public bool IsAnyPopup()
    {
        return popupStack.Count > 0;
    }

    private void Update()
    {
        if (!IsAnyPopup())
        {
            return;
        }

        if (GM.I.UIHolder.InputBlocker.activeInHierarchy)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && popupStack.Count > 0)
        {
            popupStack.Pop().Hide();
        }
    }
}
