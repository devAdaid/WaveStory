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
}