using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public WaveParameter InputWave = WaveParameter.Min;
    public string CurrentRoomId = "E";
    public Dictionary<int, List<string>> WordsByFloor = new Dictionary<int, List<string>>();
    public bool IsSoulMode = false;
    public HashSet<string> FlagIds = new HashSet<string>();
    public HashSet<string> UnlockedSouls = new HashSet<string>();
    public HashSet<string> ClearedSouls = new HashSet<string>();
    public HashSet<string> UnlockedClues = new HashSet<string>();
}
