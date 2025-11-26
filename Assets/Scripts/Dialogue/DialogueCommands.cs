using System.Collections.Generic;
using UnityEngine;

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

[DialogueCommand("Clear")]
public class ClearCommand : DialogueCommandBase
{
    private string soulId;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        soulId = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Unlock.ClearSoul(soulId);
    }
}

[DialogueCommand("Flag")]
public class FlagCommand : DialogueCommandBase
{
    private string flagId;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        flagId = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Unlock.UnlockFlag(flagId);
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
        choices.Clear();
        targetLabels.Clear();

        // parameters: [choice1, label1, choice2, label2, choice3, label3, ...]
        // 짝수 인덱스: 선택지 텍스트
        // 홀수 인덱스: 라벨

        for (int i = 0; i < parameters.Length; i += 2)
        {
            // 선택지 텍스트가 비어있으면 중단
            if (string.IsNullOrEmpty(parameters[i]))
                break;

            // 다음 파라미터(라벨)가 없으면 경고
            if (i + 1 >= parameters.Length)
            {
                Debug.LogWarning($"[ChoiceCommand] Choice '{parameters[i]}' has no label!");
                break;
            }

            string choiceText = parameters[i];
            string label = parameters[i + 1];

            // 라벨이 비어있으면 경고
            if (string.IsNullOrEmpty(label))
            {
                Debug.LogWarning($"[ChoiceCommand] Choice '{choiceText}' has empty label!");
                continue;
            }

            choices.Add(choiceText);
            targetLabels.Add(label);
        }

        // 최소 1개, 최대 3개 검증
        if (choices.Count == 0)
        {
            Debug.LogError("[ChoiceCommand] No valid choices found!");
        }
        else if (choices.Count > 3)
        {
            Debug.LogWarning($"[ChoiceCommand] Too many choices ({choices.Count}). Only first 3 will be used.");
            choices = choices.GetRange(0, 3);
            targetLabels = targetLabels.GetRange(0, 3);
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        if (choices.Count == 0)
        {
            Debug.LogError("[ChoiceCommand] Cannot execute with no choices!");
            return;
        }

        r.UI.ShowChoices(choices, (selectedIndex) =>
        {
            string label = targetLabels[selectedIndex];
            r.SelectChoice(label);
        });
    }
}

[DialogueCommand("Bg")]
public class BgCommand : DialogueCommandBase
{
    private Sprite bgSprite;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        var roomId = parameters[0];
        if (StaticDataHolder.I.TryGetRoom(roomId, out var roomData))
        {
            bgSprite = roomData.SoulSprite;
        }
        else
        {
            bgSprite = null;
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.SetBg(bgSprite);
    }
}

[DialogueCommand("Char")]
public class CharCommand : DialogueCommandBase
{
    private Sprite sprite;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        sprite = Resources.Load<Sprite>($"Portrait/{parameters[0]}");
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.SetPortrait(sprite);
    }
}

[DialogueCommand("HideChar")]
public class HideCharCommand : DialogueCommandBase
{
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.HidePortrait();
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
[DialogueCommand("If")]
public class IfCommand : DialogueCommandBase
{
    private string flagName;
    private string trueLabelName;
    private string falseLabelName;

    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        if (parameters.Length < 3)
        {
            Debug.LogError($"[JumpIfCommand] Requires 3 parameters: flagName, trueLabel, falseLabel. Got {parameters.Length}");
            return;
        }

        flagName = parameters[0];
        trueLabelName = parameters[1];
        falseLabelName = parameters[2];

        if (string.IsNullOrEmpty(flagName))
        {
            Debug.LogError("[JumpIfCommand] Flag name cannot be empty!");
        }
        if (string.IsNullOrEmpty(trueLabelName))
        {
            Debug.LogError("[JumpIfCommand] True label name cannot be empty!");
        }
        if (string.IsNullOrEmpty(falseLabelName))
        {
            Debug.LogError("[JumpIfCommand] False label name cannot be empty!");
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            Debug.LogError("[JumpIfCommand] Cannot execute with empty flag name!");
            return;
        }

        bool flagValue = r.Unlock.HasFlag(flagName) || r.Unlock.IsUnlockedSoul(flagName);
        string targetLabel = flagValue ? trueLabelName : falseLabelName;

        if (string.IsNullOrEmpty(targetLabel))
        {
            Debug.LogError($"[JumpIfCommand] Target label is empty (flag '{flagName}' = {flagValue})");
            return;
        }

        r.JumpToLabel(targetLabel);
    }
}

[DialogueCommand("PauseBgm")]
public class PauseBgmCommand : DialogueCommandBase
{
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Audio.Pause();
    }
}


[DialogueCommand("ResumeBgm")]
public class ResumeBgmCommand : DialogueCommandBase
{
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Audio.Resume();
    }
}

[DialogueCommand("StopBgm")]
public class StopBgmCommand : DialogueCommandBase
{
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Audio.FadeOutBgm(0.5f);
    }
}

[DialogueCommand("Bgm")]
public class BgmCommand : DialogueCommandBase
{
    private string clipName;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        clipName = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Audio.PlayBgm(clipName);
    }
}

[DialogueCommand("Sfx")]
public class SfxCommand : DialogueCommandBase
{
    private string clipName;
    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        clipName = parameters[0];
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.Audio.PlaySfxOneShot(clipName);
    }
}

[DialogueCommand("Shake")]
public class ShakeCommand : DialogueCommandBase
{
    private float duration;
    private float intensity;

    public override bool IsWaitingInput => false; // 흔들림 동안 대기하려면 true

    public override void Initialize(string[] parameters)
    {
        // Param1: duration (초), Param2: intensity (강도)
        // 기본값: 0.5초, 강도 0.3
        duration = parameters.Length > 0 && !string.IsNullOrEmpty(parameters[0])
            ? float.Parse(parameters[0])
            : 0.3f;

        intensity = parameters.Length > 1 && !string.IsNullOrEmpty(parameters[1])
            ? float.Parse(parameters[1])
            : 5f;

        if (duration <= 0)
        {
            Debug.LogWarning("[ShakeCommand] Duration must be positive. Using default 0.5s");
            duration = 0.3f;
        }

        if (intensity <= 0)
        {
            Debug.LogWarning("[ShakeCommand] Intensity must be positive. Using default 0.3");
            intensity = 5f;
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        r.UI.ShakeScreen(duration, intensity);
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