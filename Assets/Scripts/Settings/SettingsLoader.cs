using UnityEngine;
using UnityEngine.SceneManagement;

public static class SettingsLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoad()
    {
        //InstantiateSettings(SceneManager.GetActiveScene());

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InstantiateSettings(scene);
    }

    private static void InstantiateSettings(Scene activeScene)
    {
        GameObject settings = GameObject.Find("SettingsUI");

        if (settings)
        {
            // 이미 만들어져 있으면 또 만들지는 않아야 한다.
        }
        else
        {
            GameObject canvas = GameObject.Find("Canvas");
            settings = Object.Instantiate(Resources.Load<GameObject>("Settings/SettingsUI"), canvas ? canvas.transform : null);
            if (!settings)
            {
                Debug.LogError("Cannot find Settings from Resources");
                return;
            }

            settings.name = "SettingsUI";
            settings.GetComponent<SettingsUI>().Initialize();
            settings.GetComponent<SettingsUI>().Hide();
        }
        
        // 여기까지 왔다면 settings는 유효해야 한다.
        // 특정 씬에서만 작동하도록 하자.
        settings.SetActive(activeScene.name is "Main" or "Ending");
    }
}