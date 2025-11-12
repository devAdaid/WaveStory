public class RoomContext
{
    public string CurrentRoomId { get; private set; }

    public WaveContext[] PreviewWaves = new WaveContext[3];

    public RoomContext()
    {
        CurrentRoomId = string.Empty;

        //TODO
        PreviewWaves[0] = new WaveContext(new WaveParameter(WaveType.Sin, 5, 5));
        PreviewWaves[1] = new WaveContext(new WaveParameter(WaveType.PingPong, 8, 3));
        PreviewWaves[2] = new WaveContext(WaveParameter.Invalid);
    }

    public void SetCurrentRoom(string roomId)
    {
        CurrentRoomId = roomId;

        //TODO
    }
}
