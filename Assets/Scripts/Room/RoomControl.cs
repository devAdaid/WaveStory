using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomControl : MonoBehaviour
{
    [field: SerializeField]
    public RoomData RoomData { get; private set; }

    [SerializeField]
    private Image bgImage;

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
        bgImage.sprite = isSoulMode ? RoomData.SoulSprite : RoomData.RealSprite;

        foreach (var interactable in interactables)
        {
            interactable.OnSoulModeChange(isSoulMode);
        }
    }

    public void ApplyUnlocks(Dictionary<string, SoulState> soulStates, HashSet<string> flags)
    {
        foreach (var soul in souls)
        {
            soul.Apply(soulStates[soul.SoulData.Id]);
        }
    }
}
