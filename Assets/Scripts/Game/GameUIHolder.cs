using UnityEngine;

public class GameUIHolder : MonoBehaviour
{
    [SerializeField]
    private WaveControlUI waveControlUI;
    [SerializeField]
    private WordInventoryUI wordInventoryUI;

    public void Initialize(GM context)
    {
        waveControlUI.SetPresenter(new WaveControlPresenter(context.InputWave, context.Room, waveControlUI));
        wordInventoryUI.SetPresenter(new WordInventoryPresenter(context.WordInventory));

        foreach (var ui in gameObject.GetComponentsInChildren<UIBase>(true))
        {
            ui.Initialize();
        }
    }
}
