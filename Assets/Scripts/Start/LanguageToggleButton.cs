using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageToggleButton : MonoBehaviour
{
    [SerializeField]
    private string localeCode;

    [SerializeField]
    public Button Button;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite onSprite;
    [SerializeField]
    private Sprite offSprite;

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Color onTextColor;
    [SerializeField]
    private Color offTextColor;

    private Action<string> clickCallback;

    public void Initialize(Action<string> callback)
    {
        clickCallback = callback;
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        clickCallback.Invoke(localeCode);
    }

    public void Apply(string currentLocale)
    {
        var isSelected = localeCode == currentLocale;
        image.sprite = isSelected ? onSprite : offSprite;
        text.color = isSelected ? onTextColor : offTextColor;
    }
}
