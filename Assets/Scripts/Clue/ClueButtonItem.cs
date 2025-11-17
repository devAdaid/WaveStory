using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueButtonItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private Button button;

    private ClueData clueData;
    private bool isUnlocked;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Apply(ClueData clueData, bool isUnlocked)
    {
        this.clueData = clueData;
        this.isUnlocked = isUnlocked;
        nameText.text = isUnlocked ? clueData.Title : "???";
    }

    private void OnClick()
    {
        if (isUnlocked)
        {
            GM.I.UIHolder.ClueUI.MoveToClue(clueData);
        }
    }
}
