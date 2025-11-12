using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>, IMonoSingleton
{
    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource sfxSource;
    private Dictionary<string, AudioClip> clipMap = new();
    private AudioClip currentClip;
    private Coroutine fadeCoroutine;

    public void Initialize()
    {
        if (!bgmSource)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }
        if (!sfxSource)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        var clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (var clip in clips)
        {
            clipMap[clip.name] = clip;
            if (clip.name == "Title")
            {
                clip.LoadAudioData();
            }
        }
    }

    public void PlayBgm(AudioClip clip)
    {
        if (bgmSource.isPlaying && currentClip == clip)
        {
            return;
        }

        StopBgmFade();

        bgmSource.clip = clip;
        bgmSource.Play();
        currentClip = clip;
    }

    public void PlayBgm(string clipName)
    {
        PlayBgm(GetClip(clipName));
    }

    public void Pause()
    {
        bgmSource.Pause();
    }

    public void Resume()
    {
        bgmSource.Play();
    }

    public void StopBgm()
    {
        StopBgmFade();
        bgmSource.Stop();
    }

    public void FadeOutBgm(float duration)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    public void StopBgmFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        bgmSource.volume = 1f;
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();
        fadeCoroutine = null;

        bgmSource.volume = 1f;
    }

    public void PlaySfxOneShot(AudioClip audioClip)
    {
        sfxSource.PlayOneShot(audioClip);
    }

    public void PlaySfxOneShot(string clipName)
    {
        PlaySfxOneShot(GetClip(clipName));
    }

    public AudioClip GetClip(string name)
    {
        if (clipMap.TryGetValue(name, out var clip))
        {
            return clip;
        }
        return null;
    }
}