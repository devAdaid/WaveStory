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
        var word1String = Word1.DisplayText.GetLocalizedString();
        var word2String = Word2.DisplayText.GetLocalizedString();

        if (LocalizationSettings.SelectedLocale.Identifier.Code == "ko-KR")
        {
            return word1String + word2String;
        }
        else
        {
            return $"{word1String} {word2String}";
        }
    }

    public LocalizedString GetDisplayName()
    {
        var word1String = Word1.DisplayText.GetLocalizedString();
        var word2String = Word2.DisplayText.GetLocalizedString();

        var displayNameString = new LocalizedString("Message", "People_Display_Name_Format");

        displayNameString.Arguments = new object[]
        {
            word1String,
            word2String,
        };

        return displayNameString;
    }
}
