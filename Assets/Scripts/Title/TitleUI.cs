using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoSingleton<TitleUI>, IMonoSingleton
{
    [SerializeField]
    private Image dimmed;

    [SerializeField]
    private WaveParameter answerParameter;

    [SerializeField]
    private WaveRenderer previewRenderer;

    [SerializeField]
    private WaveRenderer inputRenderer;

    [SerializeField]
    private WaveControlUI waveControlUI;

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

    private WaveContext inputContext;
    private WaveContext previewContext;
    private WordInventoryContext wordInventoryContext;

    private bool isWordCorrect;

    IEnumerator Start()
    {
        inputContext = new WaveContext(WaveParameter.Min);
        previewContext = new WaveContext(answerParameter);
        wordInventoryContext = new WordInventoryContext();

        wordInventoryContext.Add("Title_Wave");
        wordInventoryContext.Add("Title_Last");

        AudioManager.I.PlayBgm("Noise");

        waveControlUI.SetPresenter(new WavePresenter(inputContext, waveControlUI));
        previewRenderer.SetPresenter(new WavePresenter(previewContext, previewRenderer));
        inputRenderer.SetPresenter(new WavePresenter(inputContext, inputRenderer));
        wordInventoryUI.SetPresenter(new WordInventoryPresenter(wordInventoryContext));

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

    public void Initialize()
    {
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
}
