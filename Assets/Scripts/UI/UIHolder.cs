using UnityEngine;

public class UIHolder : MonoBehaviour
{
    public void Initialize()
    {
        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>(true))
        {
            ui.Initialize();
        }
    }
}
