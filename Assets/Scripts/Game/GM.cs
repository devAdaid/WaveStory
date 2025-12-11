using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
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
    private LocalizedString saveMessage;

    public static PlayerSaveData LoadedData = null;

    public void Initialize()
    {
        // Title 씬에서 Settings UI때문에 GM이 생성되는데, 그것 이외에는 쓸모가 없다.
        // 바로 리턴한다.
        if (SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }

        if (LoadedData == null)
        {
            LoadedData = new();
            uiHolder.FirstBagUI.Show();
        }
        
        InputWave = new WaveContext(LoadedData.InputWave);
        Room = new RoomContext(LoadedData.CurrentRoomId);
        WordInventory = new WordInventoryContext(LoadedData.WordsByFloor);
        SoulMode = new SoulModeContext(LoadedData.IsSoulMode);
        Unlock = new UnlockContext(LoadedData.FlagIds, LoadedData.UnlockedSouls, LoadedData.ClearedSouls, LoadedData.UnlockedClues);

        uiHolder.Initialize(this);
        uiHolder.DimmedUI.StartFadeIn(0.5f);

        AudioManager.I.PlayBgm("Air");
    }

    public void SaveCurrentData()
    {
        var playerData = new PlayerSaveData
        {
            InputWave = InputWave.WaveParameter,
            CurrentRoomId = Room.CurrentRoomId,
            WordsByFloor = WordInventory.WordsByFloor,
            IsSoulMode = SoulMode.IsSoulMode,
            FlagIds = Unlock.FlagIds,
            UnlockedSouls = Unlock.UnlockedSouls,
            ClearedSouls = Unlock.ClearedSouls,
            UnlockedClues = Unlock.UnlockedClues
        };
        SaveDataUtility.SavePlayerData(playerData);
        uiHolder.AlarmUI.ShowAlarm(saveMessage);
    }
}
