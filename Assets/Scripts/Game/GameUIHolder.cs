using UnityEngine;
using WaveStory.Interference;

public class GameUIHolder : MonoBehaviour
{
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
    public DialogueUI DialogueUI;
    [field: SerializeField]
    public InterferencePuzzlePresenter InterferencePuzzlePresenter;

    public void Initialize(GM context)
    {
        WaveControlUI.SetPresenter(new WaveControlPresenter(context.InputWave, context.Room, WaveControlUI));
        WordInventoryUI.SetPresenter(new WordInventoryPresenter(context.WordInventory, WordInventoryUI));
        WordInputUI.SetPresenter(new WordInputPresenter(context.Room, context.InputWave, context.Unlock, WordInputUI));
        RoomUI.SetPresenter(new RoomPresenter(context.Room, context.SoulMode, context.Unlock, RoomUI));
        ClueUI.SetPresenter(new CluePresenter(context.WordInventory, ClueUI));

        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>(true))
        {
            ui.Initialize();
        }
    }
}
