public class WordInputPresenter : IPresenter
{
    private RoomContext room;
    private WaveContext wave;
    private UnlockContext unlock;
    private WordInputUI ui;

    public WordInputPresenter(RoomContext room, WaveContext wave, UnlockContext unlock, WordInputUI ui)
    {
        this.room = room;
        this.wave = wave;
        this.unlock = unlock;
        this.ui = ui;
    }

    public bool ProcessInput(string wordId1, string wordId2)
    {
        foreach (var soul in room.CurrentRoomData.Souls)
        {
            if (soul.WaveParameter == wave.WaveParameter
                && soul.Word1.Id == wordId1
                && soul.Word2.Id == wordId2)
            {
                unlock.UnlockSoul(soul.Id);
                return true;
            }
        }

        return false;
    }
}
