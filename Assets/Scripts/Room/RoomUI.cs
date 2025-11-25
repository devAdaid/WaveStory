using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class RoomUI : UIBase, IView<RoomPresenter>
{
    [SerializeField]
    private TMP_Text roomNameText;

    private Dictionary<string, RoomControl> roomMap = new Dictionary<string, RoomControl>();

    private RoomPresenter presenter;

    private string currentRoomId => currentRoomData.Id;
    private RoomData currentRoomData;
    private bool isSoulMode;
    private UnlockState state;
    private LocalizeStringEvent roomNameLocalizeEvent;

    public void SetPresenter(RoomPresenter presenter)
    {
        this.presenter = presenter;
        currentRoomData = presenter.GetCurrentRoomData();
        isSoulMode = presenter.GetIsSoulMode();
        state = presenter.GetUnlockState();
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
        roomMap[currentRoomId].Apply(isSoulMode, state);

        SetRoomNameText(roomMap[currentRoomId].RoomData);
    }

    public void ApplySoulMode(bool isSoulMode)
    {
        this.isSoulMode = isSoulMode;
        roomMap[currentRoomId].Apply(isSoulMode, state);
    }

    public void ApplyRoomData(RoomData roomData)
    {
        roomMap[currentRoomId].gameObject.SetActive(false);
        currentRoomData = roomData;

        roomMap[currentRoomId].gameObject.SetActive(true);
        roomMap[currentRoomId].Apply(isSoulMode, state);

        SetRoomNameText(roomData);
    }

    private void SetRoomNameText(RoomData roomData)
    {
        if (roomNameLocalizeEvent != null)
        {
            Destroy(roomNameLocalizeEvent);
        }
        TextHelper.SetLocalizedText(roomNameText, roomData.DisplayName, ref roomNameLocalizeEvent);
    }

    public void ApplyUnlockState(UnlockState state)
    {
        this.state = state;
        roomMap[currentRoomId].Apply(isSoulMode, state);
    }

    public void ChangeRoom(RoomData roomData)
    {
        presenter.ChangeRoom(roomData);
    }
}
