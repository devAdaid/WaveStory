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

    private SoulState state;

    public override void OnInteract()
    {
        Debug.Log(state);

        if (state == SoulState.Unlocked)
        {
            GM.I.UIHolder.DialogueUI.PlayDialogue(SoulData.DialogueOnUnlocked);
        }
        else if (state == SoulState.Cleared)
        {
            GM.I.UIHolder.DialogueUI.PlayDialogue(SoulData.DialogueOnCleared);
        }
        else if (state == SoulState.Locked)
        {
            GM.I.UIHolder.AlarmUI.ShowAlarm($"이 영혼과 대화하려면 이름을 맞춰야 한다.");
        }
    }

    protected override void ApplyUnlock(Dictionary<string, SoulState> soulStates, HashSet<string> flags)
    {
        if (soulStates.TryGetValue(SoulData.Id, out var soulState))
        {
            this.state = soulState;
            image.sprite = (state == SoulState.Locked ? SoulData.LockedSprite : SoulData.UnlockedSprite);
        }
    }
}
