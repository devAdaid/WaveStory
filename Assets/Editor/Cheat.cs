using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class Cheat
{
    [MenuItem("Cheat/층별 저승사자 대화 스킵")]
    private static void ClearSajaDialogue()
    {
        GM.I.Unlock.UnlockFlag("Saja_Meet");
        GM.I.Unlock.UnlockFlag("Saja_Corridor_2_Talk");
        GM.I.UIHolder.AlarmUI.ShowAlarm("층별 저승사자 대화 스킵 치트 완료");
    }

    [MenuItem("Cheat/모든 단서 획득")]
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
        GM.I.UIHolder.AlarmUI.ShowAlarm("모든 단서 획득 치트 완료");
    }

    [MenuItem("Cheat/모든 영혼 해금")]
    private static void UnlockAllSouls()
    {
        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            GM.I.Unlock.UnlockSoul(soulId);
        }
        GM.I.UIHolder.AlarmUI.ShowAlarm("모든 영혼 해금 치트 완료");
    }

    [MenuItem("Cheat/모든 영혼 클리어")]
    private static void ClearAllSouls()
    {
        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            GM.I.Unlock.ClearSoul(soulId);
        }
        GM.I.UIHolder.AlarmUI.ShowAlarm("모든 영혼 클리어 치트 완료");
    }
}
#endif