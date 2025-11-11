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

    public void Initialize()
    {
        var clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (var clip in clips)
        {
            clipMap[clip.name] = clip;
        }
    }

    public void PlayBgm(AudioClip clip)
    {
        if (bgmSource.isPlaying && currentClip == clip)
        {
            return;
        }

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
        bgmSource.Stop();
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
