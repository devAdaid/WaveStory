using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomUI : UIBase, IView<RoomPresenter>
{
    [SerializeField]
    private TMP_Text roomNameText;

    [SerializeField]
    private SoulModeButton soulModeButton;

    private Dictionary<string, RoomControl> roomMap = new Dictionary<string, RoomControl>();

    private RoomPresenter presenter;

    private string currentRoomId => currentRoomData.Id;
    private RoomData currentRoomData;
    private bool isSoulMode;

    public void SetPresenter(RoomPresenter presenter)
    {
        this.presenter = presenter;
        currentRoomData = presenter.GetCurrentRoomData();
        isSoulMode = presenter.GetIsSoulMode();
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
        roomMap[currentRoomId].SetSoulMode(isSoulMode);

        //TODO
        roomNameText.text = currentRoomId;

        soulModeButton.Button.onClick.AddListener(() =>
        {
            presenter.ChangeSoulMode(!isSoulMode);
        });

        soulModeButton.ApplySoulMode(isSoulMode);
    }

    public void ApplySoulMode(bool isSoulMode)
    {
        this.isSoulMode = isSoulMode;
        roomMap[currentRoomId].SetSoulMode(isSoulMode);

        soulModeButton.ApplySoulMode(isSoulMode);
    }

    public void ApplyRoomData(RoomData roomData)
    {
        roomMap[currentRoomId].gameObject.SetActive(false);
        currentRoomData = roomData;
        roomMap[currentRoomId].gameObject.SetActive(true);
        roomMap[currentRoomId].SetSoulMode(isSoulMode);
        roomNameText.text = roomData.DisplayName;
    }

    public void ChangeRoom(RoomData roomData)
    {
        presenter.ChangeRoom(roomData);
    }
}
