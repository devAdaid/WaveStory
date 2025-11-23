using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "SoulData", menuName = "Scriptable Objects/SoulData")]
public class SoulData : ScriptableObject
{
    public string Id => name;
    public string DisplayName;
    public WaveParameter WaveParameter;
    public LocalizedString HearingText;
    public WordData Word1;
    public WordData Word2;
    public Sprite LockedSprite;
    public Sprite UnlockedSprite;
    public DialogueTable DialogueTable;
}
