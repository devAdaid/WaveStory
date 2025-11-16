using UnityEngine;

public class GameUIHolder : MonoBehaviour
{
    [field: SerializeField]
    public WaveControlUI WaveControlUI;
    [field: SerializeField]
    public WordInventoryUI WordInventoryUI;
    [field: SerializeField]
    public RoomUI RoomUI;

    public void Initialize(GM context)
    {
        WaveControlUI.SetPresenter(new WaveControlPresenter(context.InputWave, context.Room, WaveControlUI));
        WordInventoryUI.SetPresenter(new WordInventoryPresenter(context.WordInventory));
        RoomUI.SetPresenter(new RoomPresenter(context.Room, context.SoulMode, RoomUI));

        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>(true))
        {
            ui.Initialize();
        }
    }
}
