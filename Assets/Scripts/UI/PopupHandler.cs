using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        // Title 씬인 경우에는 GM.I가 텅 빈 채로 존재한다. Settings UI 팝업 때문에 만들어질 수 밖에 없는 것 같으므로 임시로 조건 추가
        if (SceneManager.GetActiveScene().name != "Title" && GM.I.UIHolder.InputBlocker.activeInHierarchy)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && popupStack.Count > 0)
        {
            popupStack.Pop().Hide();
        }
    }
}
