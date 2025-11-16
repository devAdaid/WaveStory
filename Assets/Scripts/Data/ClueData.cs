using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ScriptableObject
{
    [TextArea]
    public string Text;

    public List<WordData> UnlockWords;
}
