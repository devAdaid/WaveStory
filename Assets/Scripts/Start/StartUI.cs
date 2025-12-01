using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    [SerializeField] private List<LanguageToggleButton> languageButtons;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Image fadeImage;

    private const float FadeDuration = .5f;

    private string localeCode;

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

        localeCode = GetDefaultLocale();
        SetLocale(localeCode);

        confirmButton.onClick.AddListener(Confirm);

        foreach (var languageButton in languageButtons)
        {
            languageButton.Initialize(SelectLanguage);
            languageButton.Apply(localeCode);
        }
    }

    private void SelectLanguage(string localeCode)
    {
        this.localeCode = localeCode;
        foreach (var languageButton in languageButtons)
        {
            languageButton.Apply(localeCode);
        }
        SetLocale(localeCode);
    }

    private void Confirm()
    {
        foreach (var languageButton in languageButtons)
        {
            languageButton.Button.interactable = false;
        }
        StartCoroutine(FadeOutAndLoadTitle(this.localeCode));
    }

    private IEnumerator FadeOutAndLoadTitle(string localeCode)
    {
        SetLocale(localeCode);
        PlayerPrefs.SetString(LanguageCodeKey, localeCode);
        PlayerPrefs.Save();

        fadeImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / FadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;

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

    private static string GetDefaultLocale()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.CultureInfo.Name== CultureInfo.CurrentCulture.Name)
            {
                return locale.Identifier.CultureInfo.Name;
            }
        }

        return "en";
    }
}
