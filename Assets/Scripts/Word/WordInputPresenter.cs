public class WordInputPresenter : IPresenter
{
    private RoomContext roomContext;
    private WaveContext waveContext;

    public WordInputPresenter(RoomContext roomContext, WaveContext waveContext)
    {
        this.roomContext = roomContext;
        this.waveContext = waveContext;
    }

}
