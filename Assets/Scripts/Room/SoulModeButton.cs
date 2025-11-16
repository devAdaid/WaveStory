using UnityEngine;
using UnityEngine.UI;

public class SoulModeButton : MonoBehaviour
{
    public Button Button;

    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private Sprite realSprite;

    [SerializeField]
    private Sprite soulSprite;

    public void ApplySoulMode(bool isSoulMode)
    {
        iconImage.sprite = isSoulMode ? soulSprite : realSprite;
    }
}
