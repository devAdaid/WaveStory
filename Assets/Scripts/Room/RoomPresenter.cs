using System.Collections.Generic;

public class RoomPresenter : IPresenter
{
    private RoomContext room;
    private SoulModeContext soulMode;
    private UnlockContext unlock;

    private RoomUI ui;

    public RoomPresenter(RoomContext room, SoulModeContext soulMode, UnlockContext unlock, RoomUI ui)
    {
        this.room = room;
        this.soulMode = soulMode;
        this.unlock = unlock;
        this.ui = ui;
        room.OnRoomChanged.AddListener(this.OnRoomChanged);
        soulMode.OnSoulModeChanged.AddListener(this.OnSoulModeChanged);
        unlock.OnChanged.AddListener(this.OnUnlockChanged);
    }

    public RoomData GetCurrentRoomData()
    {
        return room.CurrentRoomData;
    }

    public bool GetIsSoulMode()
    {
        return soulMode.IsSoulMode;
    }

    public Dictionary<string, SoulState> GetSoulStates()
    {
        return unlock.GetSoulStates();
    }

    public HashSet<string> GetFlagIds()
    {
        return unlock.FlagIds;
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

    private void OnUnlockChanged()
    {
        ui.ApplyUnlocks(unlock.GetSoulStates(), unlock.FlagIds);
    }

    private void OnSoulModeChanged(bool isSoulMode)
    {
        ui.ApplySoulMode(isSoulMode);
    }
}
