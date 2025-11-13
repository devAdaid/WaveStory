using System.Collections.Generic;
using System.Linq;

public class WordInventoryPresenter : IPresenter
{
    private WordInventoryContext context;

    public WordInventoryPresenter(WordInventoryContext context)
    {
        this.context = context;
    }

    public List<string> GetWordIds()
    {
        return context.WordIds.ToList();
    }
}
