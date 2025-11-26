using TMPro;
using Unity.VisualScripting;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public static class TextHelper
{
    public static void SetLocalizedText(TMP_Text tmpText, LocalizedString localizedString, ref LocalizeStringEvent localizeStringEvent)
    {
        localizeStringEvent = tmpText.gameObject.GetOrAddComponent<LocalizeStringEvent>();
        localizeStringEvent.StringReference = localizedString;

        localizeStringEvent.OnUpdateString.AddListener((value) => tmpText.text = value);
    }
}
