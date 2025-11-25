using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    private static readonly string LANGUAGE_CODE_KEY = "Language_Code";

    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button koreanButton;
    [SerializeField] private Button englishButton;

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        if (PlayerPrefs.HasKey(LANGUAGE_CODE_KEY))
        {
            string savedCode = PlayerPrefs.GetString(LANGUAGE_CODE_KEY);
            SetLocale(savedCode);
            SceneManager.LoadScene("Title");
            yield break;
        }

        languagePanel.SetActive(true);
        koreanButton.onClick.AddListener(() => SelectLanguage("ko-KR"));
        englishButton.onClick.AddListener(() => SelectLanguage("en"));
    }

    private void SelectLanguage(string localeCode)
    {
        SetLocale(localeCode);
        PlayerPrefs.SetString(LANGUAGE_CODE_KEY, localeCode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Title");
    }

    private void SetLocale(string localeCode)
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
    }

    private static void OnLocaleChanged(Locale locale)
    {
        if (locale != null)
        {
            PlayerPrefs.SetString(LANGUAGE_CODE_KEY, locale.Identifier.Code);
            PlayerPrefs.Save();
        }
    }
}
