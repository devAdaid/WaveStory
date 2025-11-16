using UnityEngine.Events;

public class RoomContext
{
    public string CurrentRoomId { get; private set; }

    public WaveParameter[] PreviewWaves = new WaveParameter[3];
    public UnityEvent<string> OnRoomChanged = new();

    public RoomContext()
    {
        //TODO
        CurrentRoomId = "C_1";

        PreviewWaves[0] = new WaveParameter(WaveType.Sin, 5, 5);
        PreviewWaves[1] = new WaveParameter(WaveType.PingPong, 8, 3);
        PreviewWaves[2] = WaveParameter.Invalid;
    }

    public void SetCurrentRoom(string roomId)
    {
        CurrentRoomId = roomId;
        OnRoomChanged.Invoke(roomId);
    }
}
