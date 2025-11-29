using UnityEngine;
using UnityEngine.Localization;

public class RoomMoveInteractable : InteractableBase
{
    [SerializeField]
    private RoomData targetRoomData;

    [SerializeField]
    private LocalizedString myNotInteractableMessage;

    protected override LocalizedString notInteractableMessage => myNotInteractableMessage;

    protected override InteractableType interactableType => InteractableType.Always;

    public override void OnInteract()
    {
        AudioManager.I.PlaySfxOneShot("Footstep");
        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(() => GM.I.UIHolder.RoomUI.ChangeRoom(targetRoomData));
    }

    protected override LocalizedString GetTooltipText()
    {
        return targetRoomData.DisplayName;
    }
}
