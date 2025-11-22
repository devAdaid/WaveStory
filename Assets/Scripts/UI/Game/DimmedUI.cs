using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DimmedUI : UIBase
{
    [SerializeField]
    private Image dimmedImage;

    private Coroutine fadeCoroutine;

    protected override void InitializeInternal()
    {
    }

    public void StartFadeOut(float fadeTime = 0.2f)
    {
        // 기존에 실행 중인 페이드가 있다면 중지
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOut(fadeTime));
    }

    public void StartFadeIn(float fadeTime = 0.2f)
    {
        // 기존에 실행 중인 페이드가 있다면 중지
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeIn(fadeTime));
    }

    public void StartFadeOutInSequence(Action callback, float fadeTime = 0.1f, float delayTime = 0.1f)
    {
        // 기존에 실행 중인 페이드가 있다면 중지
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutInSequence(fadeTime, delayTime, callback));
    }

    private IEnumerator FadeOutInSequence(float fadeTime, float delayTime, Action callback)
    {
        // 1. FadeOut 실행
        yield return StartCoroutine(FadeOut(fadeTime));

        // 2. 콜백 실행
        callback?.Invoke();

        // 3. Delay 대기
        yield return new WaitForSeconds(delayTime);

        // 4. FadeIn 실행
        yield return StartCoroutine(FadeIn(fadeTime));

        fadeCoroutine = null;
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        float elapsedTime = 0f;
        Color color = dimmedImage.color;
        float startAlpha = color.a;

        dimmedImage.gameObject.SetActive(true);
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeTime);
            dimmedImage.color = color;
            yield return null;
        }

        // 최종값 보장
        color.a = 1f;
        dimmedImage.color = color;
    }

    private IEnumerator FadeIn(float fadeTime)
    {
        float elapsedTime = 0f;
        Color color = dimmedImage.color;
        float startAlpha = color.a;

        dimmedImage.gameObject.SetActive(true);
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeTime);
            dimmedImage.color = color;
            yield return null;
        }

        // 최종값 보장
        color.a = 0f;
        dimmedImage.color = color;
        dimmedImage.gameObject.SetActive(false);
    }
}