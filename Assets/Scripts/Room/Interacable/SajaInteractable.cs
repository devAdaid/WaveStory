using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

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

    [SerializeField]
    private LocalizedString tooltipText;

    private UnlockState state;

    protected override InteractableType interactableType => InteractableType.OnlySoulMode;
    protected override TooltipType TooltipType => TooltipType.Talk;

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

    protected override LocalizedString GetTooltipText()
    {
        return tooltipText;
    }
}
