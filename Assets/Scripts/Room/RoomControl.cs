using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomControl : MonoBehaviour
{
    [field: SerializeField]
    public RoomData RoomData { get; private set; }

    [SerializeField]
    private Image bgImage;

    [SerializeField]
    private Sprite realSprite;

    [SerializeField]
    private Sprite soulSprite;

    private List<InteractableBase> interactables = new();
    private List<SoulInteractable> souls = new();

    public void Initialize()
    {
        foreach (var interactable in gameObject.GetComponentsInChildren<InteractableBase>(true))
        {
            interactables.Add(interactable);

            if (interactable is SoulInteractable soul)
            {
                souls.Add(soul);
            }
        }
    }

    public void SetSoulMode(bool isSoulMode)
    {
        bgImage.sprite = isSoulMode ? soulSprite : realSprite;

        foreach (var interactable in interactables)
        {
            interactable.OnSoulModeChange(isSoulMode);
        }
    }

    public void ApplyUnlocks(HashSet<string> unlockedSouls, HashSet<string> flags)
    {
        foreach (var soul in souls)
        {
            soul.Apply(unlockedSouls.Contains(soul.SoulData.Id));
        }
    }
}
