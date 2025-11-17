using UnityEngine;

public class RoomMoveInteractable : InteractableBase
{
    [SerializeField]
    private RoomData targetRoomData;

    [SerializeField]

    public override void OnInteract()
    {
        GM.I.UIHolder.RoomUI.ChangeRoom(targetRoomData);
    }
}
