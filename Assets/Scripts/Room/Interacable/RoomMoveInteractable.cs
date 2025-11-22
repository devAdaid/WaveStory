using UnityEngine;

public class RoomMoveInteractable : InteractableBase
{
    [SerializeField]
    private RoomData targetRoomData;

    [SerializeField]

    public override void OnInteract()
    {
        AudioManager.I.PlaySfxOneShot("Footstep");
        GM.I.UIHolder.DimmedUI.StartFadeOutInSequence(() => GM.I.UIHolder.RoomUI.ChangeRoom(targetRoomData));
    }
}
