using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelText;

    [SerializeField]
    private Button button;

    private string wordId;
    private Action<string> onClick;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Apply(string wordId, Action<string> onClick)
    {
        this.wordId = wordId;
        this.onClick = onClick;

        if (StaticDataHolder.I.TryGetWord(wordId, out var wordData))
        {
            labelText.text = wordData.DisplayText;
        }
    }

    public void OnClick()
    {
        onClick?.Invoke(wordId);
        AudioManager.I.PlaySfxOneShot("Select");
    }
}
