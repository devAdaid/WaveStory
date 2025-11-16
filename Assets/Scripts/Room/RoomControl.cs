using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomControl : MonoBehaviour
{
    [SerializeField]
    public string RoomId;

    [SerializeField]
    private Image bgImage;

    [SerializeField]
    private Sprite realSprite;

    [SerializeField]
    private Sprite soulSprite;

    private List<InteractableBase> interactables = new();

    public void Initialize()
    {
        foreach (var interactable in gameObject.GetComponentsInChildren<InteractableBase>(true))
        {
            interactables.Add(interactable);
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
}
