using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
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
    private CanvasGroup frame;

    [SerializeField]
    private CanvasGroup ui;

    private WaveContext inputContext;
    private WaveContext previewContext;

    IEnumerator Start()
    {
        inputContext = new WaveContext(WaveParameter.Min);
        previewContext = new WaveContext(answerParameter);

        AudioManager.I.PlayBgm("Noise");

        waveControlUI.SetPresenter(new WavePresenter(inputContext, waveControlUI));
        previewRenderer.SetPresenter(new WavePresenter(previewContext, previewRenderer));
        inputRenderer.SetPresenter(new WavePresenter(inputContext, inputRenderer));

        waveControlUI.Initialize();
        previewRenderer.Initialize();
        inputRenderer.Initialize();

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

        waveControlUI.SetChangeBlock(true);

        dimmed.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        var frameFadeTime = 1f;
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

    IEnumerator FadeLineRenderer(LineRenderer lineRenderer)
    {
        var fadeTime = 1f;
        var step = Time.deltaTime / fadeTime;
        var t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            var c = lineRenderer.material.color;
            c.a = alpha;
            lineRenderer.material.SetColor("_Color", c);

            t += step;
            yield return null;
        }
        {
            Color c = lineRenderer.startColor;
            c.a = 1f;
            lineRenderer.startColor = c;
            c = lineRenderer.endColor;
            c.a = 1f;
            lineRenderer.endColor = c;
        }
    }
}
