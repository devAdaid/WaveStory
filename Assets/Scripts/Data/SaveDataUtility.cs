using Newtonsoft.Json;
using UnityEngine;

public static class SaveDataUtility
{
    private static readonly string SAVE_FILE_NAME = "PlayerSaveData";
    private static readonly string SAVE_KEY = "SAVE";

    public static void SavePlayerData(PlayerSaveData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    public static PlayerSaveData LoadPlayerData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            var data = JsonConvert.DeserializeObject<PlayerSaveData>(json);
            return data;
        }

        return null;
    }

    public static bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}
