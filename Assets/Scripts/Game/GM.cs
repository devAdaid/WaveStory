using UnityEngine;

public class GM : MonoSingleton<GM>, IMonoSingleton
{
    public WaveContext InputWave { get; private set; }
    public RoomContext Room { get; private set; }
    public WordInventoryContext WordInventory { get; private set; }
    public SoulModeContext SoulMode { get; private set; }
    public UnlockContext Unlock { get; private set; }

    [SerializeField]
    private GameUIHolder uiHolder;
    public GameUIHolder UIHolder => this.uiHolder;

    public void Initialize()
    {
        InputWave = new WaveContext(WaveParameter.Min);
        Room = new RoomContext();
        WordInventory = new WordInventoryContext();
        SoulMode = new SoulModeContext();
        Unlock = new UnlockContext();

        uiHolder.Initialize(this);
        uiHolder.DimmedUI.StartFadeIn(0.5f);
    }
}
