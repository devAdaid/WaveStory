using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private Sprite highlightSprite;

    private SpriteRenderer spriteRender;
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (!spriteRender)
            {
                spriteRender = GetComponent<SpriteRenderer>();
            }

            return spriteRender;
        }
    }

    private void OnMouseDown()
    {
        OnInteract();
    }

    private void OnMouseEnter()
    {
        SpriteRenderer.sprite = highlightSprite;
    }

    private void OnMouseExit()
    {
        SpriteRenderer.sprite = sprite;
    }

    public abstract void OnInteract();
}
