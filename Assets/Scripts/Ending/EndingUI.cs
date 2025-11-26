using TMPro;
using UnityEngine;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private EndingWaveElement[] endingWaveElements;
    [SerializeField] private EndingSoul endingSoul;
    [SerializeField] private WaveHand waveHand;
    [SerializeField] private TextMeshProUGUI creditText;
    [SerializeField] private float targetX = 100f;

    private async void Start()
    {
        foreach (var element in endingWaveElements)
        {
            waveHand.ResetState();

            endingSoul.SetSprite(element.sprite);
            creditText.text = element.creditText.GetLocalizedString();

            await endingSoul.StartElement(targetX);
        }
    }
}
