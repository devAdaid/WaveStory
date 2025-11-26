using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ScriptableObject
{
    public string Id => this.name;
    public int Order;
    public int Floor;
    public LocalizedString Title;

    public LocalizedString Text;

    public List<WordData> UnlockWords;
}
