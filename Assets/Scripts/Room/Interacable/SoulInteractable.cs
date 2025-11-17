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
    }

    public void Apply(SoulState state)
    {
        this.state = state;
        image.sprite = (state == SoulState.Locked ? SoulData.LockedSprite : SoulData.UnlockedSprite);
    }
}
