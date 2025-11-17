using System.Collections.Generic;
using UnityEngine.Events;

public class RoomContext
{
    public string CurrentRoomId => CurrentRoomData.Id;
    public RoomData CurrentRoomData { get; private set; }
    public UnityEvent<RoomData> OnRoomChanged = new();

    public RoomContext()
    {
        if (StaticDataHolder.I.TryGetRoom("E", out var roomData))
        {
            CurrentRoomData = roomData;
        }
    }

    public void SetCurrentRoom(RoomData roomData)
    {
        CurrentRoomData = roomData;
        OnRoomChanged.Invoke(roomData);
    }

    public List<WaveParameter> GetPreviewParameters()
    {
        var results = new List<WaveParameter>();
        foreach (var soul in CurrentRoomData.Souls)
        {
            results.Add(soul.WaveParameter);
        }
        return results;
    }
}
