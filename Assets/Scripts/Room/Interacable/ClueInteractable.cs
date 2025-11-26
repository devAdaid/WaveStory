using UnityEngine;
using UnityEngine.Localization;

public class ClueInteractable : InteractableBase
{
    protected override LocalizedString notInteractableMessage => new LocalizedString("Message", "Cannot_Inspect_Clue_SoulMode");

    [SerializeField]
    private ClueData clueData;
    protected override InteractableType interactableType => InteractableType.OnlyRealMode;

    public override void OnInteract()
    {
        AudioManager.I.PlaySfxOneShot("Paper");
        GM.I.UIHolder.ClueUI.OpenClue(clueData);
    }

    protected override string GetTooltipText()
    {
        return clueData.Title.GetLocalizedStringAsync().WaitForCompletion();
    }
}
