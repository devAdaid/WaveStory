using UnityEditor;
using UnityEngine;

public static class Cheat
{
    [MenuItem("Cheat/¸ðµç ´Ü¼­ È¹µæ")]
    private static void UnlockAllClues()
    {
        foreach (var clue in StaticDataHolder.I.ClueDataList)
        {
            GM.I.Unlock.UnlockClue(clue.Id);
            foreach (var word in clue.UnlockWords)
            {
                GM.I.WordInventory.Add(word.Id, clue.Floor);
            }
        }
    }

    [MenuItem("Cheat/¸ðµç ¿µÈ¥ ÇØ±Ý")]
    private static void UnlockAllSouls()
    {
        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            GM.I.Unlock.UnlockSoul(soulId);
        }
    }

    [MenuItem("Cheat/¸ðµç ¿µÈ¥ Å¬¸®¾î")]
    private static void ClearAllSouls()
    {
        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            GM.I.Unlock.ClearSoul(soulId);
        }
    }
}
