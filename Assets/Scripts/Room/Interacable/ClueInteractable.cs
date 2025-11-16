using UnityEngine;

public class ClueInteractable : InteractableBase
{
    [SerializeField]
    private string clueId;

    public override void OnInteract()
    {
        GM.I.UIHolder.ClueUI.OpenClue(clueId);
    }
}
