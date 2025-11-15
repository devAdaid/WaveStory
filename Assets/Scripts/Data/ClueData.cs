using UnityEngine;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ScriptableObject
{
    [TextArea]
    public string Text;
}
