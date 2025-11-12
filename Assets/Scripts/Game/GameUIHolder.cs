using UnityEngine;

public class GameUIHolder : MonoBehaviour
{
    [SerializeField]
    private WaveControlUI waveControlUI;

    [SerializeField]
    private WaveRenderer inputWaveRenderer;

    [SerializeField]
    private WaveRenderer[] previewWaveRenderers;

    public void Initialize(GameContextHolder context)
    {
        waveControlUI.SetPresenter(new WavePresenter(context.InputWave, waveControlUI));
        inputWaveRenderer.SetPresenter(new WavePresenter(context.InputWave, inputWaveRenderer));

        previewWaveRenderers[0].SetPresenter(new WavePresenter(context.Room.PreviewWaves[0], previewWaveRenderers[0]));
        previewWaveRenderers[1].SetPresenter(new WavePresenter(context.Room.PreviewWaves[1], previewWaveRenderers[1]));
        previewWaveRenderers[2].SetPresenter(new WavePresenter(context.Room.PreviewWaves[2], previewWaveRenderers[2]));
    }
}
