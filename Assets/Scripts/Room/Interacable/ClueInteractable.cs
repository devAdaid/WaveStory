using UnityEngine;

public class ClueInteractable : InteractableBase
{
    [SerializeField]
    private ClueData clueData;

    public override void OnInteract()
    {
        AudioManager.I.PlaySfxOneShot("Paper");
        GM.I.UIHolder.ClueUI.OpenClue(clueData);
    }
}
