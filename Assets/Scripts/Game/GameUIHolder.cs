using UnityEngine;

public class GameUIHolder : MonoBehaviour
{
    [field: SerializeField]
    public MainHudUI MainHudUI;
    [field: SerializeField]
    public WaveControlUI WaveControlUI;
    [field: SerializeField]
    public WordInventoryUI WordInventoryUI;
    [field: SerializeField]
    public WordInputUI WordInputUI;
    [field: SerializeField]
    public RoomUI RoomUI;
    [field: SerializeField]
    public ClueUI ClueUI;
    [field: SerializeField]
    public SettingsUI SettingsUI;
    [field: SerializeField]
    public DialogueUI DialogueUI;
    [field: SerializeField]
    public TooltipUI TooltilUI;
    [field: SerializeField]
    public DimmedUI DimmedUI;
    [field: SerializeField]
    public AlarmUI AlarmUI;
    [field: SerializeField]
    public GameObject InputBlocker;

    public void Initialize(GM context)
    {
        MainHudUI.SetPresenter(new MainHudPresenter(context.SoulMode, MainHudUI));
        WaveControlUI.SetPresenter(new WaveControlPresenter(context.InputWave, context.Room, context.Unlock, WaveControlUI));
        WordInventoryUI.SetPresenter(new WordInventoryPresenter(context.WordInventory, context.Unlock, WordInventoryUI));
        WordInputUI.SetPresenter(new WordInputPresenter(context.Room, context.InputWave, context.Unlock, WordInputUI));
        RoomUI.SetPresenter(new RoomPresenter(context.Room, context.SoulMode, context.Unlock, RoomUI));
        ClueUI.SetPresenter(new CluePresenter(context.WordInventory, context.Unlock, ClueUI));

        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>(true))
        {
            ui.Initialize();
        }
    }

    public bool IsShowMainHud()
    {
        return !PopupHandler.I.IsAnyPopup()
            && !DialogueUI.IsActive;
    }
}
