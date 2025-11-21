using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomUI : UIBase, IView<RoomPresenter>
{
    [SerializeField]
    private TMP_Text roomNameText;

    private Dictionary<string, RoomControl> roomMap = new Dictionary<string, RoomControl>();

    private RoomPresenter presenter;

    private string currentRoomId => currentRoomData.Id;
    private RoomData currentRoomData;
    private bool isSoulMode;
    private Dictionary<string, SoulState> unlockedSouls;
    private HashSet<string> flags;

    public void SetPresenter(RoomPresenter presenter)
    {
        this.presenter = presenter;
        currentRoomData = presenter.GetCurrentRoomData();
        isSoulMode = presenter.GetIsSoulMode();
        unlockedSouls = presenter.GetSoulStates();
        flags = presenter.GetFlagIds();
    }

    protected override void InitializeInternal()
    {
        foreach (var room in gameObject.GetComponentsInChildren<RoomControl>(true))
        {
            roomMap.Add(room.RoomData.Id, room);
            room.Initialize();
            room.gameObject.SetActive(false);
        }
        roomMap[currentRoomId].gameObject.SetActive(true);
        roomMap[currentRoomId].Apply(isSoulMode, unlockedSouls, flags);

        roomNameText.text = roomMap[currentRoomId].RoomData.DisplayName;
    }

    public void ApplySoulMode(bool isSoulMode)
    {
        this.isSoulMode = isSoulMode;
        roomMap[currentRoomId].Apply(isSoulMode, unlockedSouls, flags);
    }

    public void ApplyRoomData(RoomData roomData)
    {
        roomMap[currentRoomId].gameObject.SetActive(false);
        currentRoomData = roomData;

        roomMap[currentRoomId].gameObject.SetActive(true);
        roomMap[currentRoomId].Apply(isSoulMode, unlockedSouls, flags);

        roomNameText.text = roomData.DisplayName;
    }

    public void ApplyUnlocks(Dictionary<string, SoulState> soulStates, HashSet<string> flags)
    {
        this.unlockedSouls = soulStates;
        roomMap[currentRoomId].Apply(isSoulMode, soulStates, flags);
    }

    public void ChangeRoom(RoomData roomData)
    {
        presenter.ChangeRoom(roomData);
    }
}
