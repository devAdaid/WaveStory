using System.Collections.Generic;
using System.Linq;

public class WordInventoryPresenter : IPresenter
{
    private WordInventoryContext wordInventory;
    private UnlockContext unlock;
    private WordInventoryUI ui;

    public WordInventoryPresenter(WordInventoryContext wordInventory, UnlockContext unloclk, WordInventoryUI ui)
    {
        this.wordInventory = wordInventory;
        this.unlock = unloclk;
        this.ui = ui;
        wordInventory.OnWordAdded.AddListener(OnWordAdded);
        unlock.OnFlagAndSoulChanged.AddListener(() => ui.Apply(GetCurrentFloorWordIds()));
    }

    public List<string> GetCurrentFloorWordIds()
    {
        return wordInventory.GetWords(unlock.GetCurrentProgressFloor());
    }

    private void OnWordAdded(string wordId)
    {
        ui.Apply(GetCurrentFloorWordIds());
    }
}
