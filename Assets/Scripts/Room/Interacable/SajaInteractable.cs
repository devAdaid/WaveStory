using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SajaDialogue
{
    public UnlockCondition Condition;
    public TextAsset DialogueText;
}

public class SajaInteractable : InteractableBase
{
    [SerializeField]
    private List<SajaDialogue> dialogues;

    private Dictionary<string, SoulState> soulStates;
    private HashSet<string> flags;

    public override void OnInteract()
    {
        if (TryGetSatisfyDialogue(out var dialogueText))
        {
            GM.I.UIHolder.DialogueUI.PlayDialogue(dialogueText);
        }
    }

    protected override void ApplyUnlock(Dictionary<string, SoulState> soulStates, HashSet<string> flags)
    {
        this.soulStates = soulStates;
        this.flags = flags;
    }

    private bool TryGetSatisfyDialogue(out TextAsset dialogueText)
    {
        foreach (var d in dialogues)
        {
            if (d.Condition.SatisfyCondition(soulStates, flags))
            {
                dialogueText = d.DialogueText;
                return true;
            }
        }

        dialogueText = null;
        return false;
    }
}
