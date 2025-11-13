using UnityEngine;

public enum WordType
{
    First,
    Second,
}

[CreateAssetMenu(fileName = "WordData", menuName = "Scriptable Objects/WordData")]
public class WordData : ScriptableObject
{
    public string Id;
    public WordType Type;
    public string DisplayText;
}
