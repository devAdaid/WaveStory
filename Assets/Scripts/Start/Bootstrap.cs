using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public static readonly string LanguageCodeKey = "Language_Code";
    
    [SerializeField] private StartUI startUI;
    
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
        }
        else
        {
            startUI.gameObject.SetActive(true);
        }
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
        if (locale == null) return;
        
        PlayerPrefs.SetString(LanguageCodeKey, locale.Identifier.Code);
        PlayerPrefs.Save();
    }

    public static string GetDefaultLocale()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales.Where(locale => locale.Identifier.CultureInfo.Name == CultureInfo.CurrentCulture.Name))
        {
            return locale.Identifier.CultureInfo.Name;
        }

        return "en";
    }
}
