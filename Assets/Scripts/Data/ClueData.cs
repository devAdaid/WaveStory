using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ScriptableObject
{
    public string Id => this.name;
    public int Order;
    [TextArea]
    public string Title;

    public LocalizedString Text;

    public List<WordData> UnlockWords;
}
