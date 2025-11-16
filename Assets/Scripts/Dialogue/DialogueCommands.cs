using System.Collections.Generic;

public interface IDialogueCommand
{
    void Execute(IDialogueRuntime r);
    bool IsWaitingInput { get; }
    void Initialize(string[] parameters);
}
public abstract class DialogueCommandBase : IDialogueCommand
{
    public abstract bool IsWaitingInput { get; }
    public abstract void Execute(IDialogueRuntime r);
    public abstract void Initialize(string[] parameters);
}

[DialogueCommand("Text")]
public class TextCommand : DialogueCommandBase
{
    private string text;
    public override bool IsWaitingInput => true;

    public override void Initialize(string[] parameters)
    {
        text = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.ShowText(text);
    }
}

[DialogueCommand("Speaker")]
public class SpeakerCommand : DialogueCommandBase
{
    private string speakerName;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        speakerName = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.SetSpeakerName(speakerName);
    }
}

[DialogueCommand("Choice")]
public class ChoiceCommand : DialogueCommandBase
{
    private List<string> choices = new List<string>();
    private List<string> targetLabels = new List<string>();
    public override bool IsWaitingInput => true;

    public override void Initialize(string[] parameters)
    {
        // parameters: [choice1, choice2, label1, label2, ...]
        int halfIndex = parameters.Length / 2;

        for (int i = 0; i < halfIndex; i++)
        {
            if (!string.IsNullOrEmpty(parameters[i]))
                choices.Add(parameters[i]);
        }

        for (int i = halfIndex; i < parameters.Length; i++)
        {
            if (!string.IsNullOrEmpty(parameters[i]))
                targetLabels.Add(parameters[i]);
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.ShowChoices(choices, (selectedIndex) =>
        {
            string label = targetLabels[selectedIndex];
            r.JumpToLabel(label);
        });
    }
}

[DialogueCommand("WindowVisible")]
public class WindowVisibleCommand : DialogueCommandBase
{
    private bool visible;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        visible = bool.Parse(parameters[0]);
    }

    public override void Execute(IDialogueRuntime r)
    {
        if (visible)
            r.UI.Show();
        else
            r.UI.Hide();
    }
}

[DialogueCommand("Jump")]
public class JumpCommand : DialogueCommandBase
{
    private string targetLabel;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        targetLabel = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.JumpToLabel(targetLabel);
    }
}

[DialogueCommand("End")]
public class EndCommand : DialogueCommandBase
{
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        // 파라미터 필요 없음
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.EndDialogue();
    }
}