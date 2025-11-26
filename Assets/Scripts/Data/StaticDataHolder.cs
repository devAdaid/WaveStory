using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticDataHolder : PersistentSingleton<StaticDataHolder>, IMonoSingleton
{
    public WaveConstant WaveConstant;
    private Dictionary<string, WordData> wordMap = new();
    private Dictionary<string, ClueData> clueMap = new();
    private Dictionary<string, SoulData> soulMap = new();
    private Dictionary<string, RoomData> roomMap = new();
    private Dictionary<int, List<SoulData>> soulsByFloor = new();
    public List<ClueData> ClueDataList = new();

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
        ClueDataList = clueMap.Values.OrderBy(x => x.Order).ToList();

        var souls = Resources.LoadAll<SoulData>("Data/Souls");
        foreach (var soul in souls)
        {
            soulMap[soul.name] = soul;
        }

        var rooms = Resources.LoadAll<RoomData>("Data/Rooms");
        foreach (var room in rooms)
        {
            roomMap[room.name] = room;

            if (!soulsByFloor.ContainsKey(room.Floor))
            {
                soulsByFloor[room.Floor] = new List<SoulData>();
            }

            foreach (var soul in room.Souls)
            {
                soulsByFloor[room.Floor].Add(soul);
            }
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

    public List<string> GetAllSoulIds()
    {
        return soulMap.Keys.ToList();
    }

    public bool TryGetRoom(string id, out RoomData room)
    {
        return roomMap.TryGetValue(id, out room);
    }

    public List<SoulData> GetSoulsInFloor(int floor)
    {
        if (soulsByFloor.TryGetValue(floor, out var souls))
        {
            return souls;
        }

        return new List<SoulData>();
    }

    public List<RoomData> GetRoomsInFloor(int floor)
    {
        var result = new List<RoomData>();
        foreach (var room in roomMap.Values)
        {
            if (room.Floor == floor)
            {
                result.Add(room);
            }
        }
        return result;
    }
}
