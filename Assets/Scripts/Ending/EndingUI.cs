using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private EndingWaveElement[] endingWaveElements;
    [SerializeField] private EndingSoul endingSoul;
    [SerializeField] private WaveHand waveHand;
    [SerializeField] private TextMeshProUGUI creditText;
    [SerializeField] private float targetX = 100f;

    private LocalizeStringEvent creditTextLocalizeStringEvent;

    private async void Start()
    {
        foreach (var element in endingWaveElements)
        {
            waveHand.ResetState();

            endingSoul.SetSprite(element.sprite);
            creditText.text = element.creditText.GetLocalizedString();
            
            TextHelper.SetLocalizedText(creditText, element.creditText, ref creditTextLocalizeStringEvent);
            creditTextLocalizeStringEvent.RefreshString();

            await endingSoul.StartElement(targetX);
        }
    }
}
