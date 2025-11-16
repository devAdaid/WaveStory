using System.Collections.Generic;
using UnityEngine;

public class StaticDataHolder : PersistentSingleton<StaticDataHolder>, IMonoSingleton
{
    public WaveConstant WaveConstant;
    private Dictionary<string, WordData> wordMap = new();
    private Dictionary<string, ClueData> clueMap = new();
    private Dictionary<string, SoulData> soulMap = new();
    private Dictionary<string, RoomData> roomMap = new();

    public void Initialize()
    {
        WaveConstant = Resources.Load<WaveConstant>("Data/WaveConstant");

        var words = Resources.LoadAll<WordData>("Data/Words");
        foreach (var word in words)
        {
            wordMap[word.name] = word;
        }

        var clues = Resources.LoadAll<ClueData>("Data/Clues");
        foreach (var clue in clues)
        {
            clueMap[clue.name] = clue;
        }

        var souls = Resources.LoadAll<SoulData>("Data/Souls");
        foreach (var soul in souls)
        {
            soulMap[soul.name] = soul;
        }

        var rooms = Resources.LoadAll<RoomData>("Data/Rooms");
        foreach (var room in rooms)
        {
            roomMap[room.name] = room;
        }
    }

    public bool TryGetWord(string id, out WordData word)
    {
        return wordMap.TryGetValue(id, out word);
    }

    public bool TryGetClue(string id, out ClueData clue)
    {
        return clueMap.TryGetValue(id, out clue);
    }

    public bool TryGetSoul(string id, out SoulData soul)
    {
        return soulMap.TryGetValue(id, out soul);
    }

    public bool TryGetRoom(string id, out RoomData room)
    {
        return roomMap.TryGetValue(id, out room);
    }
}
