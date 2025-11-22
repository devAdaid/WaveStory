using System.Collections.Generic;
using UnityEngine;
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

    private UnlockState state;
    private SoulState soulState;

    public override void OnInteract()
    {
        if (SoulData.DialogueTable.TryGetDialogue(state, out var dialogue))
        {
            GM.I.UIHolder.DialogueUI.PlayDialogue(dialogue);
        }
        
        if (soulState == SoulState.Locked)
        {
            GM.I.UIHolder.AlarmUI.ShowAlarm($"이 영혼과 대화하려면 대응하는 파동 이름을 맞춰야 한다.");
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
}
