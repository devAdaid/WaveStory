using UnityEngine;

public class BgmContext
{
    private AudioClip backgroundBgm;

    public BgmContext(string backgroundBgmName)
    {
        backgroundBgm = AudioManager.I.GetClip(backgroundBgmName);
    }

    public void SetCurrentBgm(string backgroundBgmName)
    {
        backgroundBgm = AudioManager.I.GetClip(backgroundBgmName);
        AudioManager.I.PlayBgm(backgroundBgm);
    }
}
