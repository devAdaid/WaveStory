public class CluePresenter : IPresenter
{
    private readonly WordInventoryContext wordInventory;
    private ClueUI ui;

    public CluePresenter(WordInventoryContext wordInventory, ClueUI ui)
    {
        this.wordInventory = wordInventory;
        this.ui = ui;
    }

    public void AdddWord(string wordId)
    {
        wordInventory.Add(wordId);
    }
}
