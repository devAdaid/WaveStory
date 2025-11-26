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
    private DialogueTable dialogueTable;

    private UnlockState state;

    protected override InteractableType interactableType => InteractableType.OnlySoulMode;

    public override void OnInteract()
    {
        if (dialogueTable.TryGetDialogue(state, out var dialogue))
        {
            GM.I.UIHolder.DialogueUI.PlayDialogue(dialogue);
        }
    }

    protected override void ApplyUnlock(UnlockState state)
    {
        this.state = state;
    }

    protected override string GetTooltipText()
    {
        return "Àú½Â»çÀÚ";
    }
}
