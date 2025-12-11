using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public static class SaveDataUtility
{
    private static readonly string SAVE_FILE_NAME = "PlayerSaveData";
    private static readonly string SAVE_KEY = "SAVE";
    public static string SaveFilePath => GetSavePath(SAVE_FILE_NAME);

    public static void SavePlayerData(PlayerSaveData data)
    {
        var savePath = SaveFilePath;
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        PlayerPrefs.SetString(SAVE_KEY, json);
        Debug.Log("Saved path: " + savePath);
    }

    public static PlayerSaveData LoadPlayerData()
    {
        var savePath = SaveFilePath;
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

    private static string GetSavePath(string filename)
    {
        var path = Application.persistentDataPath;
#if UNITY_WEBGL && !UNITY_EDITOR
         path = "/idbfs/the-last-wave";
          if (!Directory.Exists(path)) {
             Directory.CreateDirectory(path);
             Debug.Log("Creating save directory: " + path);
         }
#endif
        var result = Path.Combine(path, filename);
        return result;
    }
}
