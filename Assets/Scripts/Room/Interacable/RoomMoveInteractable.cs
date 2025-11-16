using UnityEngine;

public class RoomMoveInteractable : InteractableBase
{
    [SerializeField]
    private string targetRoomId;

    public override void OnInteract()
    {
        GM.I.UIHolder.RoomUI.ChangeRoomId(targetRoomId);
    }
}
