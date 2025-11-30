using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public static readonly string LanguageCodeKey = "Language_Code";

    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button koreanButton;
    [SerializeField] private Button englishButton;

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        if (PlayerPrefs.HasKey(LanguageCodeKey))
        {
            string savedCode = PlayerPrefs.GetString(LanguageCodeKey);
            SetLocale(savedCode);
            SceneManager.LoadScene("Title");
            yield break;
        }

        languagePanel.SetActive(true);
        koreanButton.onClick.AddListener(() => SelectLanguage("ko-KR"));
        englishButton.onClick.AddListener(() => SelectLanguage("en"));
    }

    private static void SelectLanguage(string localeCode)
    {
        SetLocale(localeCode);
        PlayerPrefs.SetString(LanguageCodeKey, localeCode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Title");
    }

    public static void SetLocale(string localeCode)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales.Where(locale => locale.Identifier.Code == localeCode))
        {
            LocalizationSettings.SelectedLocale = locale;
            break;
        }
    }

    public static void OnLocaleChanged(Locale locale)
    {
        if (locale != null)
        {
            PlayerPrefs.SetString(LanguageCodeKey, locale.Identifier.Code);
            PlayerPrefs.Save();
        }
    }
}
