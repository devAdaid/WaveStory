using UnityEngine;

public class GameUIHolder : MonoBehaviour
{
    [field: SerializeField]
    public WaveControlUI WaveControlUI;
    [field: SerializeField]
    public WordInventoryUI WordInventoryUI;
    [field: SerializeField]
    public RoomUI RoomUI;
    [field: SerializeField]
    public ClueUI ClueUI;

    public void Initialize(GM context)
    {
        WaveControlUI.SetPresenter(new WaveControlPresenter(context.InputWave, context.Room, WaveControlUI));
        WordInventoryUI.SetPresenter(new WordInventoryPresenter(context.WordInventory, WordInventoryUI));
        RoomUI.SetPresenter(new RoomPresenter(context.Room, context.SoulMode, RoomUI));
        ClueUI.SetPresenter(new CluePresenter(context.WordInventory, ClueUI));

        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>(true))
        {
            ui.Initialize();
        }
    }
}
