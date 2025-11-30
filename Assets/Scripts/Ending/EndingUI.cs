using RedBlueGames.Tools.TextTyper;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private float introDelay = 1f;
    [SerializeField] private float introTypeWaitDelay = 2f;
    [SerializeField] private GameObject intro;
    [SerializeField] private CanvasGroup dimmed;
    [SerializeField] private TextTyper introTextTyper;
    [SerializeField] private LocalizedString[] introStrings;
    [SerializeField] private float dimmedFadeDuration = 1f;

    [SerializeField] private EndingWaveElement[] endingWaveElements;
    [SerializeField] private EndingSoul endingSoul;
    [SerializeField] private WaveHand waveHand;
    [SerializeField] private TextMeshProUGUI creditText;
    [SerializeField] private float targetX = 100f;
    [SerializeField] private RectTransform messageGroup;
    [SerializeField] private CanvasGroup foreground;
    [SerializeField] private float foregroundDelay = 1f;
    [SerializeField] private float foregroundFadeDuration = 1f;
    [SerializeField] private float creditTextFadeDuration = 0.5f;

    private async void Start()
    {
        creditText.text = " ";
        creditText.color = new Color(creditText.color.r, creditText.color.g, creditText.color.b, 0f);
        foreground.alpha = 0f;

        AudioManager.I.PlayBgm("Ending");

        dimmed.alpha = 0f;

        await Awaitable.WaitForSecondsAsync(introDelay);
        foreach (var str in introStrings)
        {
            await TypeText(str, introTypeWaitDelay);
        }

        await FadeDimmedAsync(0f, 1f);

        intro.SetActive(false);

        await FadeDimmedAsync(1f, 0f);

        await Awaitable.WaitForSecondsAsync(foregroundDelay);
        await FadeForegroundAsync(0f, 1f);

        foreach (var element in endingWaveElements)
        {
            waveHand.ResetState();

            endingSoul.SetSprite(element.sprite);

            TextHelper.SetLocalizedText(creditText, element.creditText);

            await FadeCreditTextAsync(0f, 1f);
            await endingSoul.StartElement(targetX);

            var disappearTask = endingSoul.DisappearLeft();
            var fadeOutTask = FadeCreditTextAsync(1f, 0f);
            await disappearTask;
            await fadeOutTask;
        }

        await Awaitable.WaitForSecondsAsync(foregroundDelay);
        await FadeForegroundAsync(1f, 0f);
    }

    private async Awaitable TypeText(LocalizedString str, float waitDelay)
    {
        var text = str.GetLocalizedStringAsync().WaitForCompletion();
        introTextTyper.TypeText(text);
        while (introTextTyper.IsTyping)
        {
            await Awaitable.NextFrameAsync();
        }

        await Awaitable.WaitForSecondsAsync(waitDelay);
    }

    private async Awaitable FadeDimmedAsync(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < dimmedFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dimmedFadeDuration);
            dimmed.alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);
            await Awaitable.NextFrameAsync();
        }

        dimmed.alpha = endAlpha;
    }

    private async Awaitable FadeForegroundAsync(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < foregroundFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / foregroundFadeDuration);
            foreground.alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);
            await Awaitable.NextFrameAsync();
        }

        foreground.alpha = endAlpha;
    }

    private async Awaitable FadeCreditTextAsync(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = creditText.color;

        while (elapsed < creditTextFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / creditTextFadeDuration);
            color.a = Mathf.SmoothStep(startAlpha, endAlpha, t);
            creditText.color = color;
            await Awaitable.NextFrameAsync();
        }

        color.a = endAlpha;
        creditText.color = color;
    }
}
