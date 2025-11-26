using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "SoulData", menuName = "Scriptable Objects/SoulData")]
public class SoulData : ScriptableObject
{
    public string Id => name;
    public bool IsStaticSoul;
    public WaveParameter WaveParameter;
    public LocalizedString HearingText;
    public WordData Word1;
    public WordData Word2;
    public Sprite LockedSprite;
    public Sprite UnlockedSprite;
    public DialogueTable DialogueTable;

    public string GetLocalizedDisplayName()
    {
        var word1String = Word1.DisplayText.GetLocalizedStringAsync().WaitForCompletion();
        var word2String = Word2.DisplayText.GetLocalizedStringAsync().WaitForCompletion();

        if (LocalizationSettings.SelectedLocale.Identifier.Code == "ko-KR")
        {
            return word1String + word2String;
        }
        else
        {
            return $"{word1String} {word2String}";
        }
    }
}
