using UnityEngine;
using UnityEngine.Localization;

public enum WordType
{
    First,
    Second,
}

[CreateAssetMenu(fileName = "WordData", menuName = "Scriptable Objects/WordData")]
public class WordData : ScriptableObject
{
    public string Id => this.name;
    public WordType Type;
    public LocalizedString DisplayText;
}
