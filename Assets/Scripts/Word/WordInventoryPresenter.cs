using System.Collections.Generic;
using System.Linq;

public class WordInventoryPresenter : IPresenter
{
    private WordInventoryContext context;
    private WordInventoryUI ui;

    public WordInventoryPresenter(WordInventoryContext context, WordInventoryUI ui)
    {
        this.context = context;
        this.ui = ui;
        context.OnWordAdded.AddListener(OnWordAdded);
    }

    public List<string> GetWordIds()
    {
        return context.WordIds.ToList();
    }

    private void OnWordAdded(string wordId)
    {
        ui.Apply(GetWordIds());
    }
}
