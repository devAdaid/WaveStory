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
    private KnobButton[] buttons;

    [SerializeField]
    private CanvasGroup frame;

    [SerializeField]
    private CanvasGroup ui;

    IEnumerator Start()
    {
        previewRenderer.WaveController.SetParamter(answerParameter);
        inputRenderer.WaveController.SetParamter(WaveParameter.Min);

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
            t += dimmedStep;
            var alpha = Mathf.Lerp(1f, 0f, t);
            dimmed.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        dimmed.color = new Color(0f, 0f, 0f, 0f);

        yield return new WaitForSeconds(0.3f);

        dimmed.gameObject.SetActive(false);

        foreach (var button in buttons)
        {
            button.SetArrowActive(true);
        }

        while (previewRenderer.WaveController.WaveParameter != inputRenderer.WaveController.WaveParameter)
        {
            yield return null;
        }

        inputRenderer.WaveController.SetChangeBlock(true);

        dimmed.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        previewRenderer.gameObject.SetActive(false);

        var frameFadeTime = 2f;
        var frameStep = Time.deltaTime / frameFadeTime;
        t = 0f;
        while (t < 1f)
        {
            t += frameStep;
            var alpha = Mathf.Lerp(1f, 0f, t);
            frame.alpha = alpha;
            yield return null;
        }
        frame.alpha = 0f;
        frame.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        ui.gameObject.SetActive(true);

        var uiFadeTime = 2f;
        var uiStep = Time.deltaTime / uiFadeTime;
        t = 0f;
        while (t < 1f)
        {
            t += uiStep;
            var alpha = Mathf.Lerp(0f, 1f, t);
            ui.alpha = alpha;
            yield return null;
        }
        ui.alpha = 1f;
    }
}
