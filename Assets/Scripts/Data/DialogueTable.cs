using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

[System.Serializable]
public class DialogueTable
{
    public List<DialogueTableItem> Dialogues;

    public bool TryGetDialogue(UnlockState state, out TextAsset dialogue)
    {
        foreach (DialogueTableItem item in Dialogues)
        {
            if (item.Condition.IsSatisfiedBy(state))
            {
                dialogue = item.Dialogue;
                return true;
            }
        }

        dialogue = null;
        return false;
    }
}

[System.Serializable]
public class DialogueTableItem
{
    public UnlockCondition Condition;

    [FormerlySerializedAs("Dialogue")]
    public TextAsset DialogueKo;

    [System.NonSerialized] private TextAsset _dialogueEn;
    [System.NonSerialized] private bool _dialogueEnLoaded;

    private TextAsset DialogueEn
    {
        get
        {
            if (!_dialogueEnLoaded && DialogueKo != null)
            {
                _dialogueEnLoaded = true;
                string enPath = $"Dialogues_en/{DialogueKo.name}";
                _dialogueEn = Resources.Load<TextAsset>(enPath);
            }
            return _dialogueEn;
        }
    }

    public TextAsset Dialogue
    {
        get
        {
            var locale = LocalizationSettings.SelectedLocale;
            if (locale != null && locale.Identifier.Code == "ko-KR")
            {
                return DialogueKo;
            }
            return DialogueEn ?? DialogueKo;
        }
    }
}
