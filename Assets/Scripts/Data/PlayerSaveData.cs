using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Scripting;

[System.Serializable]
[Preserve]
public class PlayerSaveData
{
    [Preserve] public WaveParameter InputWave;
    [Preserve] public string CurrentRoomId;
    [Preserve] public Dictionary<int, List<string>> WordsByFloor;
    [Preserve] public bool IsSoulMode;
    [Preserve] public HashSet<string> FlagIds;
    [Preserve] public HashSet<string> UnlockedSouls;
    [Preserve] public HashSet<string> ClearedSouls;
    [Preserve] public HashSet<string> UnlockedClues;

    // 기본 생성자 추가 (중요!)
    [JsonConstructor]
    [Preserve]
    public PlayerSaveData()
    {
        InputWave = WaveParameter.Min;
        CurrentRoomId = "E";
        WordsByFloor = new Dictionary<int, List<string>>();
        IsSoulMode = false;
        FlagIds = new HashSet<string>();
        UnlockedSouls = new HashSet<string>();
        ClearedSouls = new HashSet<string>();
        UnlockedClues = new HashSet<string>();
    }
}