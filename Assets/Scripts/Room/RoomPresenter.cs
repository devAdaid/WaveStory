public class RoomPresenter : IPresenter
{
    private RoomContext room;
    private SoulModeContext soulMode;

    private RoomUI ui;

    public RoomPresenter(RoomContext room, SoulModeContext soulMode, RoomUI ui)
    {
        this.room = room;
        this.soulMode = soulMode;
        this.ui = ui;
        room.OnRoomChanged.AddListener(this.OnRoomChanged);
        soulMode.OnSoulModeChanged.AddListener(this.OnSoulModeChanged);
    }

    public RoomData GetCurrentRoomData()
    {
        return room.CurrentRoomData;
    }

    public bool GetIsSoulMode()
    {
        return soulMode.IsSoulMode;
    }

    public void ChangeSoulMode(bool isSoulMode)
    {
        if (soulMode.IsSoulMode != isSoulMode)
        {
            soulMode.SetSoulMode(isSoulMode);
        }
    }

    public void ChangeRoom(RoomData roomData)
    {
        room.SetCurrentRoom(roomData);
    }

    private void OnRoomChanged(RoomData roomData)
    {
        ui.ApplyRoomData(roomData);
    }

    private void OnSoulModeChanged(bool isSoulMode)
    {
        ui.ApplySoulMode(isSoulMode);
    }
}
