using RedBlueGames.Tools.TextTyper;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    [SerializeField] private CanvasGroup guideArrow;
    [SerializeField] private float guideArrowFadeDuration = 0.5f;
    [SerializeField] private Volume volume;
    [SerializeField] private float bloomDuration = 3f;
    [SerializeField] private CanvasGroup whiteImage;

    [SerializeField] private bool skipUntilLastSoul;

    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;

        LocalizationSettings.SelectedLocaleChanged -= StartUI.OnLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += StartUI.OnLocaleChanged;

        if (PlayerPrefs.HasKey(StartUI.LanguageCodeKey))
        {
            string savedCode = PlayerPrefs.GetString(StartUI.LanguageCodeKey);
            StartUI.SetLocale(savedCode);
        }

        intro.gameObject.SetActive(true);

        creditText.text = " ";
        creditText.color = new Color(creditText.color.r, creditText.color.g, creditText.color.b, 0f);
        foreground.alpha = 0f;

        AudioManager.I.PlayBgm("Ending1");

        dimmed.gameObject.SetActive(true);
        dimmed.alpha = 0f;

        await Awaitable.WaitForSecondsAsync(introDelay);
        foreach (var str in introStrings)
        {
            await TypeText(str, introTypeWaitDelay);
        }

        _ = FadeGuideArrowAsync(0f, 0f);

        await FadeDimmedAsync(0f, 1f);

        intro.SetActive(false);

        await FadeDimmedAsync(1f, 0f);

        await Awaitable.WaitForSecondsAsync(foregroundDelay);
        await FadeForegroundAsync(0f, 1f);

        bool isFirstElement = true;

        if (skipUntilLastSoul)
        {
            var element = endingWaveElements[^1];

            waveHand.ResetState();

            endingSoul.SetSprite(element.sprite);

            TextHelper.SetLocalizedText(creditText, element.creditText);

            await FadeCreditTextAsync(0f, 1f);
            await endingSoul.StartElement(targetX);
            waveHand.SetInputEnabled(true);
            if (isFirstElement)
            {
                _ = FadeGuideArrowAsync(0f, 1f);
            }

            await endingSoul.WaitForSuccessAsync();

            // 첫 번째 요소 완료 후 guideArrow 페이드아웃
            if (isFirstElement)
            {
                isFirstElement = false;
                _ = FadeGuideArrowAsync(1f, 0f);
            }

            AudioManager.I.PlayBgm("Ending2");
            await FadeUsingBloom();
            SceneManager.LoadScene("Ending_2");
            return;
        }

        foreach (var element in endingWaveElements)
        {
            waveHand.ResetState();

            endingSoul.SetSprite(element.sprite);

            TextHelper.SetLocalizedText(creditText, element.creditText);

            await FadeCreditTextAsync(0f, 1f);
            await endingSoul.StartElement(targetX);
            waveHand.SetInputEnabled(true);
            if (isFirstElement)
            {
                _ = FadeGuideArrowAsync(0f, 1f);
            }

            await endingSoul.WaitForSuccessAsync();

            // 첫 번째 요소 완료 후 guideArrow 페이드아웃
            if (isFirstElement)
            {
                isFirstElement = false;
                _ = FadeGuideArrowAsync(1f, 0f);
            }

            if (endingWaveElements[^1] == element)
            {
                AudioManager.I.PlayBgm("Ending2");
                await FadeUsingBloom();
                SceneManager.LoadScene("Ending_2");
                await FadeUsingBloom();
                return;
            }

            var disappearTask = endingSoul.DisappearLeft();

            AudioManager.I.PlaySfxOneShot("WaveFullCycleSuccess");

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

    private async Awaitable FadeGuideArrowAsync(float startAlpha, float endAlpha)
    {
        if (guideArrow == null) return;

        float elapsed = 0f;

        while (elapsed < guideArrowFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / guideArrowFadeDuration);
            guideArrow.alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);
            await Awaitable.NextFrameAsync();
        }

        guideArrow.alpha = endAlpha;
    }
    private async Awaitable FadeUsingBloom()
    {
        if (volume.profile.TryGet(out Bloom bloom))
        {
            float startIntensity = bloom.intensity.value;
            float targetIntensity = 15f;
            float startAlpha = whiteImage.alpha;
            float targetAlpha = 1f;
            float elapsed = 0f;

            while (elapsed < bloomDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / bloomDuration;

                bloom.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
                if (whiteImage)
                {
                    whiteImage.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                }
                else
                {
                    break;
                }

                await Awaitable.NextFrameAsync();
            }

            // 정확한 최종 값 보장
            bloom.intensity.value = targetIntensity;
            if (whiteImage)
            {
                whiteImage.alpha = targetAlpha;
            }
        }
    }
}
