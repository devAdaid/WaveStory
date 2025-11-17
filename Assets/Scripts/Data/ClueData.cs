using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ScriptableObject
{
    public string Id => this.name;
    public int Order;
    [TextArea]
    public string Title;
    [TextArea]
    public string Text;

    public List<WordData> UnlockWords;
}
