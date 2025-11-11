using System;
using System.Collections.Generic;
using UnityEngine;

public class UIHolder : MonoBehaviour
{
    [SerializeField]
    private IReadOnlyDictionary<Type, UIBase> uiMap;

    public void Initialize()
    {
        var map = new Dictionary<Type, UIBase>();
        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>())
        {
            map.Add(ui.GetType(), ui);
        }
        uiMap = map;
    }

    public T GetUI<T>(Type type) where T : UIBase
    {
        if (uiMap.TryGetValue(type, out var ui))
        {
            return ui as T;
        }

        return null;
    }
}
