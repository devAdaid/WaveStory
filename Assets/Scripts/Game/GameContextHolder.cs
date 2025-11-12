using UnityEngine;

public class GameContextHolder : MonoSingleton<GameContextHolder>, IMonoSingleton
{
    public WaveContext InputWave;
    public RoomContext Room;
    public WordInventoryContext WordInventory;
    public FlagStateContext FlagStateContext;

    [SerializeField]
    private GameUIHolder uiHolder;

    public void Initialize()
    {
        InputWave = new WaveContext(WaveParameter.Min);
        Room = new RoomContext();
        WordInventory = new WordInventoryContext();
        FlagStateContext = new FlagStateContext();

        uiHolder.Initialize(this);
    }
}
