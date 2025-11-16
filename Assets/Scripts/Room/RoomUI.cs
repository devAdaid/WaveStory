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

    private string currentRoomId;
    private bool isSoulMode;

    public void SetPresenter(RoomPresenter presenter)
    {
        this.presenter = presenter;
        currentRoomId = presenter.GetCurrentRoomId();
        isSoulMode = presenter.GetIsSoulMode();
    }

    protected override void InitializeInternal()
    {
        foreach (var room in gameObject.GetComponentsInChildren<RoomControl>(true))
        {
            roomMap.Add(room.RoomId, room);
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

    public void ApplyRoomId(string roomId)
    {
        roomMap[currentRoomId].gameObject.SetActive(false);
        this.currentRoomId = roomId;
        roomMap[currentRoomId].gameObject.SetActive(true);
        roomMap[currentRoomId].SetSoulMode(isSoulMode);
        roomNameText.text = roomId;
    }

    public void ChangeRoomId(string roomId)
    {
        presenter.ChangeRoom(roomId);
    }
}
