using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelText;

    [SerializeField]
    private Button button;

    private string wordId;
    private Action<string> onClick;
    private LocalizeStringEvent localizeStringEvent;

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
            if (localizeStringEvent != null)
            {
                Destroy(localizeStringEvent);
            }
            TextHelper.SetLocalizedText(labelText, wordData.DisplayText, ref localizeStringEvent);
        }
    }

    public void OnClick()
    {
        onClick?.Invoke(wordId);
        AudioManager.I.PlaySfxOneShot("Select");
    }
}
