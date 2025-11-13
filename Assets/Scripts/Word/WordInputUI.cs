using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordInputUI : UIBase
{
    [SerializeField]
    private TMP_Text firstWordText;

    [SerializeField]
    private TMP_Text secondWordText;

    [SerializeField]
    private Button confirmButton;

    private string wordId1;
    private string wordId2;

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }
}
