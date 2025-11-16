using UnityEngine;

public class Test_Dialogue : MonoBehaviour
{
    [SerializeField]
    private TextAsset dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GM.I.UIHolder.DialogueUI.PlayDialogue(dialogue);
    }
}
