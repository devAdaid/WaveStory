public class CluePresenter : IPresenter
{
    private readonly WordInventoryContext wordInventory;
    private readonly UnlockContext unlock;
    private ClueUI ui;

    public CluePresenter(WordInventoryContext wordInventory, UnlockContext unlock, ClueUI ui)
    {
        this.wordInventory = wordInventory;
        this.unlock = unlock;
        this.ui = ui;

        unlock.OnClueChanged.AddListener(ui.UpdateUI);
    }

    public bool AddWord(string wordId, int floor)
    {
        return wordInventory.Add(wordId, floor);
    }

    public void UnlockClue(string clueId)
    {
        unlock.UnlockClue(clueId);
    }

    public bool IsUnlocked(string clueId)
    {
        return unlock.IsUnlockedClue(clueId);
    }
}
