using System.Collections.Generic;
using UnityEngine;

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
    public TextAsset Dialogue;

    [System.NonSerialized] private TextAsset _dialogueEn;
    [System.NonSerialized] private bool _dialogueEnLoaded;

    public TextAsset DialogueEn
    {
        get
        {
            if (!_dialogueEnLoaded && Dialogue != null)
            {
                _dialogueEnLoaded = true;
                string enPath = $"Dialogues_en/{Dialogue.name}";
                _dialogueEn = Resources.Load<TextAsset>(enPath);
            }
            return _dialogueEn;
        }
    }
}