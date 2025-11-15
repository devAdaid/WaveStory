public class RoomContext
{
    public string CurrentRoomId { get; private set; }

    public WaveParameter[] PreviewWaves = new WaveParameter[3];

    public RoomContext()
    {
        CurrentRoomId = string.Empty;

        //TODO
        PreviewWaves[0] = new WaveParameter(WaveType.Sin, 5, 5);
        PreviewWaves[1] = new WaveParameter(WaveType.PingPong, 8, 3);
        PreviewWaves[2] = WaveParameter.Invalid;
    }

    public void SetCurrentRoom(string roomId)
    {
        CurrentRoomId = roomId;

        //TODO
    }
}
