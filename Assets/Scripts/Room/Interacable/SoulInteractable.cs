using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public enum SoulState
{
    Locked,
    Unlocked,
    Cleared,
}

public class SoulInteractable : InteractableBase
{
    [field: SerializeField]
    public SoulData SoulData { get; private set; }
    [SerializeField]
    private Image image;

    protected override InteractableType interactableType => InteractableType.OnlySoulMode;
    protected override LocalizedString notInteractableMessage => new LocalizedString("Message", "Not_Unlocked_Soul");

    private UnlockState state;
    private SoulState soulState;

    public override void OnInteract()
    {
        if (SoulData.DialogueTable.TryGetDialogue(state, out var dialogue))
        {
            GM.I.UIHolder.DialogueUI.PlayDialogue(dialogue);
        }
    }

    protected override void ApplyUnlock(UnlockState state)
    {
        this.state = state;
        if (state.SoulStates.TryGetValue(SoulData.Id, out var soulState))
        {
            this.soulState = soulState;
            image.sprite = (soulState == SoulState.Locked ? SoulData.LockedSprite : SoulData.UnlockedSprite);
        }
    }

    protected override bool IsActive(bool isSoulMode, UnlockState context)
    {
        return base.IsActive(isSoulMode, context) && soulState != SoulState.Cleared;
    }

    protected override bool IsInteractable(bool isSoulMode, UnlockState context)
    {
        return base.IsInteractable(isSoulMode, context) && soulState != SoulState.Locked;
    }

    protected override LocalizedString GetTooltipText()
    {
        return soulState == SoulState.Locked ? new LocalizedString("Message", "Hidden_Interactable_Tooltip") : SoulData.GetDisplayName();
    }
}
