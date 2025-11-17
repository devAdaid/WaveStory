using System.Collections.Generic;

public interface IDialogueRuntime
{
    public DialogueUI UI { get; }
    public AudioManager Audio { get; }
    public UnlockContext Unlock { get; }
    public Dictionary<string, int> Labels { get; }
    public int CurrentLineIndex { get; }

    public void JumpToLabel(string label);
    public void EndDialogue();
    public void ProcessNextCommand();
}