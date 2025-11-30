using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastEndingUI : MonoBehaviour
{
    public WaveRenderer Wave;
    public SoulData SoulData;

    IEnumerator Start()
    {
        AudioManager.I.PlayBgm("Ending2");
        Wave.Apply(SoulData.WaveParameter);
        yield return null;
    }

    public void PlayEnding3()
    {
        AudioManager.I.SetBgmLoop(false);
        AudioManager.I.PlayBgm("Ending3");
    }

    public void StartFadeLine()
    {
        StartCoroutine(FadeOut());
    }

    public void ToTitle()
    {
        AudioManager.I.SetBgmLoop(true);
        SceneManager.LoadScene("Title");
    }

    public IEnumerator FadeOut()
    {
        var frameFadeTime = 1.5f;
        var frameStep = Time.deltaTime / frameFadeTime;
        var t = 0f;
        while (t < 1f)
        {
            var alpha = Mathf.Lerp(1f, 0f, t);
            var c = Wave.LineRenderer.material.color;
            c.a = alpha;
            Wave.LineRenderer.material.SetColor("_Color", c);

            t += frameStep;
            yield return null;
        }
    }
}
