using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private List<LanguageToggleButton> languageButtons;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Image fadeImage;

    private const float FadeDuration = .5f;

    private string localeCode;

    private void Start()
    {
        languagePanel.SetActive(true);

        localeCode = Bootstrap.GetDefaultLocale();
        Bootstrap.SetLocale(localeCode);

        confirmButton.onClick.AddListener(Confirm);

        foreach (var languageButton in languageButtons)
        {
            languageButton.Initialize(SelectLanguage);
            languageButton.Apply(localeCode);
        }
    }

    private void SelectLanguage(string inLocaleCode)
    {
        localeCode = inLocaleCode;
        
        foreach (var languageButton in languageButtons)
        {
            languageButton.Apply(inLocaleCode);
        }
        Bootstrap.SetLocale(inLocaleCode);
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
        Bootstrap.SetLocale(localeCode);
        PlayerPrefs.SetString(Bootstrap.LanguageCodeKey, localeCode);
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
}
