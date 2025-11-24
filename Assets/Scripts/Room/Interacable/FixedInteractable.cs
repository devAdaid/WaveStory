using UnityEngine;
using UnityEngine.Localization;

public class FixedInteractable : InteractableBase
{
    protected override InteractableType interactableType => InteractableType.Always;

    public override void OnInteract()
    {
        Debug.Log("wow");
    }
}
