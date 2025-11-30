using UnityEditor;

#if UNITY_EDITOR
public static class Cheat
{
    [MenuItem("Cheat/Ãþº° Àú½Â»çÀÚ ´ëÈ­ ½ºÅµ")]
    private static void ClearSajaDialogue()
    {
        GM.I.Unlock.UnlockFlag("Saja_Meet");
        GM.I.Unlock.UnlockFlag("Saja_Corridor_1_Talk");
        GM.I.Unlock.UnlockFlag("Saja_Corridor_2_Talk");
        GM.I.Unlock.UnlockFlag("Saja_Corridor_3_Talk");
        GM.I.Unlock.UnlockFlag("Saja_Corridor_4_Talk");
        GM.I.UIHolder.AlarmUI.ShowAlarm("Ãþº° Àú½Â»çÀÚ ´ëÈ­ ½ºÅµ Ä¡Æ® ¿Ï·á");
    }

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
        GM.I.UIHolder.AlarmUI.ShowAlarm("¸ðµç ´Ü¼­ È¹µæ Ä¡Æ® ¿Ï·á");
    }

    [MenuItem("Cheat/¸ðµç ¿µÈ¥ ÇØ±Ý")]
    private static void UnlockAllSouls()
    {
        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            if (StaticDataHolder.I.TryGetSoul(soulId, out var soulData) && !soulData.IsStaticSoul)
            {
                GM.I.Unlock.UnlockSoul(soulId);
            }
        }
        GM.I.UIHolder.AlarmUI.ShowAlarm("¸ðµç ¿µÈ¥ ÇØ±Ý Ä¡Æ® ¿Ï·á");
    }

    [MenuItem("Cheat/¸ðµç ¿µÈ¥ Å¬¸®¾î")]
    private static void ClearAllSouls()
    {
        foreach (var soulId in StaticDataHolder.I.GetAllSoulIds())
        {
            if (StaticDataHolder.I.TryGetSoul(soulId, out var soulData) && !soulData.IsStaticSoul)
            {
                GM.I.Unlock.ClearSoul(soulId);
            }
        }
        GM.I.UIHolder.AlarmUI.ShowAlarm("¸ðµç ¿µÈ¥ Å¬¸®¾î Ä¡Æ® ¿Ï·á");
    }

    [MenuItem("Cheat/1Ãþ Å¬¸®¾î")]
    private static void ClearFloor1() => ClearFloor(1);

    [MenuItem("Cheat/2Ãþ Å¬¸®¾î")]
    private static void ClearFloor2() => ClearFloor(2);

    [MenuItem("Cheat/3Ãþ Å¬¸®¾î")]
    private static void ClearFloor3() => ClearFloor(3);

    [MenuItem("Cheat/4Ãþ Å¬¸®¾î")]
    private static void ClearFloor4() => ClearFloor(4);

    [MenuItem("Cheat/1Ãþ ÀÌµ¿")]
    private static void MoveFloor1() => MoveRoom("F1_C");

    [MenuItem("Cheat/2Ãþ ÀÌµ¿")]
    private static void MoveFloor2() => MoveRoom("F2_C");

    [MenuItem("Cheat/3Ãþ ÀÌµ¿")]
    private static void MoveFloor3() => MoveRoom("F3_C");

    [MenuItem("Cheat/4Ãþ ÀÌµ¿")]
    private static void MoveFloor4() => MoveRoom("F4_C");


    private static void ClearFloor(int maxFloor)
    {
        for (int floor = 1; floor <= maxFloor; floor++)
        {
            foreach (var room in StaticDataHolder.I.GetRoomsInFloor(floor))
            {
                foreach (var soul in room.Souls)
                {
                    GM.I.Unlock.ClearSoul(soul.Id);
                }
            }

            foreach (var clue in StaticDataHolder.I.ClueDataList)
            {
                if (clue.Floor == floor)
                {
                    GM.I.Unlock.UnlockClue(clue.Id);
                }
            }

            if (floor == 1)
            {
                GM.I.Unlock.UnlockFlag("Saja_Meet");
                GM.I.Unlock.UnlockFlag("Saja_Corridor_1_Talk");
            }

            if (floor == 2)
            {
                GM.I.Unlock.UnlockFlag("Saja_Corridor_2_Talk");
            }

            if (floor == 3)
            {
                GM.I.Unlock.UnlockFlag("Saja_Corridor_3_Talk");
            }

            if (floor == 4)
            {
                GM.I.Unlock.UnlockFlag("Saja_Corridor_4_Talk");
            }
        }

        GM.I.UIHolder.AlarmUI.ShowAlarm($"{maxFloor}Ãþ Ä¡Æ® ¿Ï·á");
    }

    private static void MoveRoom(string roomId)
    {
        if (StaticDataHolder.I.TryGetRoom(roomId, out var roomData))
        {
            GM.I.Room.SetCurrentRoom(roomData);
        }
    }
}
#endif