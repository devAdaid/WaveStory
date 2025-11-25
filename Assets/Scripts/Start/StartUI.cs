using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    private static readonly string LANGUAGE_SET_KEY = "Language_Set";

    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button koreanButton;
    [SerializeField] private Button englishButton;

    private void Start()
    {
        if (PlayerPrefs.GetInt(LANGUAGE_SET_KEY, 0) == 1)
        {
            SceneManager.LoadScene("Title");
            return;
        }

        languagePanel.SetActive(true);
        koreanButton.onClick.AddListener(() => SelectLanguage("ko-KR"));
        englishButton.onClick.AddListener(() => SelectLanguage("en"));
    }

    private void SelectLanguage(string localeCode)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == localeCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }

        PlayerPrefs.SetInt(LANGUAGE_SET_KEY, 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Title");
    }
}
