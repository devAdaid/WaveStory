using UnityEngine;
using UnityEngine.UI;

public class SoulInteractable : InteractableBase
{
    [field: SerializeField]
    public SoulData SoulData { get; private set; }
    [SerializeField]
    private Image image;

    private bool isUnlocked;

    public override void OnInteract()
    {
        if (isUnlocked)
        {
            Debug.Log("Unlocked");
        }
        else
        {
            Debug.Log("Locked");
        }
    }

    public void Apply(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
        image.sprite = isUnlocked ? SoulData.UnlockedSprite : SoulData.LockedSprite;
    }
}
