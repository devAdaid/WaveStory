using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text choiceText;

    [SerializeField]
    private Button button;

    private Action onClick;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Apply(string text, Action clickCallback)
    {
        choiceText.text = text;
        onClick = clickCallback;
    }

    private void OnClick()
    {
        onClick?.Invoke();
    }
}
