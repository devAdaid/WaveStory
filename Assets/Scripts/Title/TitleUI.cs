using RedBlueGames.Tools.TextTyper;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoSingleton<TitleUI>, IMonoSingleton
{
    [SerializeField]
    private Image dimmed;

    [SerializeField]
    private WaveParameter answerParameter;

    [SerializeField]
    private WaveRenderer_Title previewRenderer;

    [SerializeField]
    private WaveRenderer_Title inputRenderer;

    [SerializeField]
    private WaveControlUI_Title waveControlUI;

    [SerializeField]
    private KnobButton[] buttons;

    [SerializeField]
    private WordInputButton wordInputButton;

    [SerializeField]
    private WordInventoryUI wordInventoryUI;

    [SerializeField]
    private WordInputUI_Title wordInputUI;

    [SerializeField]
    private CanvasGroup frame;

    [SerializeField]
    private CanvasGroup wordUIRoot;

    [SerializeField]
    private CanvasGroup wordInventoryRoot;

    [SerializeField]
    private CanvasGroup ui;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private CanvasGroup introObj;

    [SerializeField]
    private TextTyper introTyper;

    [SerializeField]
    private List<LocalizedString> introTexts;

    private WaveContext inputContext;
    private WaveContext previewContext;
    private WordInventoryContext wordInventoryContext;

    private bool isWordCorrect;

    private static readonly string TITLE_PLAYED_KEY = "Title_Played";

    public void Initialize()
    {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    IEnumerator Start()
    {
        introTyper.CharacterPrinted.AddListener(HandleCharacterPrinted);

        if (PlayerPrefs.GetInt(TITLE_PLAYED_KEY, 0) == 0)
        {
            yield return TutorialWithTitle();
        }
        else
        {
            yield return Title();
        }
    }

    private void HandleCharacterPrinted(string printedCharacter)
    {
        if (printedCharacter == " " || printedCharacter == "\n")
        {
            return;
        }

        AudioManager.I.PlaySfx("Type");
    }

    IEnumerator TutorialWithTitle()
    {
        inputContext = new WaveContext(WaveParameter.Min);
        previewContext = new WaveContext(answerParameter);
        wordInventoryContext = new WordInventoryContext(new List<string>());

        wordInventoryContext.Add("Title_Wave");
        wordInventoryContext.Add("Title_Last");

        waveControlUI.SetPresenter(new WavePresenter_Title(inputContext, waveControlUI));
        previewRenderer.SetPresenter(new WavePresenter_Title(previewContext, previewRenderer));
        inputRenderer.SetPresenter(new WavePresenter_Title(inputContext, inputRenderer));
        wordInventoryUI.SetPresenter(new WordInventoryPresenter(wordInventoryContext, wordInventoryUI));

        waveControlUI.Initialize();
        previewRenderer.Initialize();
        inputRenderer.Initialize();
        wordInventoryUI.Initialize();
        wordInputUI.Initialize();
        wordInputButton.Initialize();

        wordInventoryUI.Hide();
        wordInputButton.Hide();

        dimmed.gameObject.SetActive(true);
        ui.gameObject.SetActive(false);
        frame.gameObject.SetActive(true);

        foreach (var button in buttons)
        {
            button.SetArrowActive(false);
        }

        AudioManager.I.PlayBgm("Noise");

        var dimmedFadeTime = 1f;
        var dimmedStep = Time.deltaTime / dimmedFadeTime;
        var t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            dimmed.color = new Color(0f, 0f, 0f, alpha);
            t += dimmedStep;
            yield return null;
        }
        dimmed.color = new Color(0f, 0f, 0f, 0f);

        yield return new WaitForSeconds(0.3f);

        dimmed.gameObject.SetActive(false);

        foreach (var button in buttons)
        {
            button.SetArrowActive(true);
        }

        while (previewContext.WaveParameter != inputContext.WaveParameter)
        {
            yield return null;
        }

        AudioManager.I.PlaySfxOneShot("Correct");

        waveControlUI.SetChangeBlock(true);
        wordInputButton.Show();

        while (!isWordCorrect)
        {
            yield return null;
        }

        AudioManager.I.PlaySfxOneShot("Correct");

        dimmed.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        dimmed.gameObject.SetActive(false);

        var wordFadeTime = 1f;
        var wordStep = Time.deltaTime / wordFadeTime;
        t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            wordUIRoot.alpha = alpha;
            wordInventoryRoot.alpha = alpha;
            yield return null;
            t += wordStep;
        }
        wordUIRoot.alpha = 0f;
        wordInventoryRoot.alpha = 0f;

        yield return new WaitForSeconds(1f);

        var frameFadeTime = 1.5f;
        var frameStep = Time.deltaTime / frameFadeTime;
        t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            frame.alpha = alpha;

            var c = previewRenderer.LineRenderer.material.color;
            c.a = alpha;
            previewRenderer.LineRenderer.material.SetColor("_Color", c);

            t += frameStep;
            yield return null;
        }
        frame.alpha = 0f;
        frame.gameObject.SetActive(false);

        previewRenderer.LineRenderer.gameObject.SetActive(false);

        AudioManager.I.FadeOutBgm(1f);

        yield return new WaitForSeconds(1f);

        ui.gameObject.SetActive(true);

        PlayerPrefs.SetInt(TITLE_PLAYED_KEY, 1);
        PlayerPrefs.Save();

        AudioManager.I.PlayBgm("Title");

        var uiFadeTime = 2f;
        var uiStep = Time.deltaTime / uiFadeTime;
        t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(0f, 1f, t);
            ui.alpha = alpha;
            yield return null;
            t += uiStep;
        }
        ui.alpha = 1f;
    }

    IEnumerator Title()
    {
        inputContext = new WaveContext(answerParameter);

        waveControlUI.SetPresenter(new WavePresenter_Title(inputContext, waveControlUI));
        inputRenderer.SetPresenter(new WavePresenter_Title(inputContext, inputRenderer));

        waveControlUI.Initialize();
        inputRenderer.Initialize();
        wordInputUI.Initialize();
        wordInputButton.Initialize();

        wordInventoryUI.Hide();
        wordInputButton.Hide();

        waveControlUI.SetChangeBlock(true);
        previewRenderer.LineRenderer.gameObject.SetActive(false);
        frame.gameObject.SetActive(false);

        AudioManager.I.PlayBgm("Title");

        dimmed.gameObject.SetActive(true);
        var dimmedFadeTime = 1f;
        var dimmedStep = Time.deltaTime / dimmedFadeTime;
        var t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            dimmed.color = new Color(0f, 0f, 0f, alpha);
            t += dimmedStep;
            yield return null;
        }
        dimmed.gameObject.SetActive(false);

        ui.gameObject.SetActive(true);
        var uiFadeTime = 2f;
        var uiStep = Time.deltaTime / uiFadeTime;
        t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(0f, 1f, t);
            ui.alpha = alpha;
            yield return null;
            t += uiStep;
        }
        ui.alpha = 1f;
    }

    public void OnInput(string wordId1, string wordId2)
    {
        isWordCorrect = (wordId1 == "Title_Last" && wordId2 == "Title_Wave");

        if (!isWordCorrect)
        {
            wordInputUI.ClearAllWords();
            AudioManager.I.PlaySfxOneShot("Wrong");
        }
    }

    private void StartGame()
    {
        StartCoroutine(Intro());
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    IEnumerator Intro()
    {
        var introStrings = new List<string>();
        foreach (var lt in introTexts)
        {
            introStrings.Add(lt.GetLocalizedStringAsync().WaitForCompletion());
        }

        dimmed.gameObject.SetActive(true);

        AudioManager.I.FadeOutBgm(1f);

        var dimmedFadeTime = 0.5f;
        var dimmedStep = Time.deltaTime / dimmedFadeTime;
        var t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(0f, 1f, t);
            dimmed.color = new Color(0f, 0f, 0f, alpha);
            t += dimmedStep;
            yield return null;
        }
        dimmed.color = new Color(0f, 0f, 0f, 1f);

        introObj.gameObject.SetActive(true);

        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            dimmed.color = new Color(0f, 0f, 0f, alpha);
            t += dimmedStep;
            yield return null;
        }
        dimmed.color = new Color(0f, 0f, 0f, 0f);

        yield return new WaitForSeconds(1f);

        foreach (var s in introStrings)
        {
            introTyper.TypeText(s);

            while (introTyper.IsTyping)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(1f);

        introTyper.GetComponent<TMP_Text>().text = string.Empty;

        dimmed.color = new Color(0f, 0f, 0f, 1f);

        var uiFadeTime = 2f;
        var uiStep = Time.deltaTime / uiFadeTime;
        t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            introObj.alpha = alpha;
            yield return null;
            t += uiStep;
        }
        introObj.alpha = 0f;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Main");
    }
}
