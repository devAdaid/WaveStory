using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private RoomData defaultRoomData;

    public void Initialize()
    {
        // Title 씬에서 Settings UI때문에 GM이 생성되는데, 그것 이외에는 쓸모가 없다.
        // 바로 리턴한다.
        if (SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }
        
        InputWave = new WaveContext(WaveParameter.Min);
        Room = new RoomContext(defaultRoomData.Id);
        WordInventory = new WordInventoryContext(new ());
        SoulMode = new SoulModeContext(false);
        Unlock = new UnlockContext(new HashSet<string>(), new HashSet<string>(), new HashSet<string>(), new HashSet<string>());

        uiHolder.Initialize(this);
        uiHolder.DimmedUI.StartFadeIn(0.5f);

        AudioManager.I.PlayBgm("Air");
    }
}
