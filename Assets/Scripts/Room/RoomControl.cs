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
    private AudioClip bgmClip;

    private List<InteractableBase> interactables = new();
    private List<SoulInteractable> souls = new();

    public void Initialize()
    {
        foreach (var interactable in gameObject.GetComponentsInChildren<InteractableBase>(true))
        {
            interactables.Add(interactable);
        }
    }

    public void Apply(bool isSoulMode, UnlockState context)
    {
        bgImage.sprite = isSoulMode ? RoomData.SoulSprite : RoomData.RealSprite;
        foreach (var interactable in interactables)
        {
            interactable.Apply(isSoulMode, context);
        }

        if (bgmClip)
        {
            AudioManager.I.PlayBgm(bgmClip);
        }
    }
}
