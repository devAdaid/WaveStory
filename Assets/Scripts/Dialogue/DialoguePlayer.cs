using System;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlayer : IDialogueRuntime
{
    public DialogueUI UI => ui;
    public AudioManager Audio => AudioManager.I;
    public UnlockContext Unlock => unlock;
    public Dictionary<string, int> Labels => labels;
    public int CurrentLineIndex => currentCommandIndex;

    private DialogueUI ui;
    private DialogueCommandFactory factory;
    private UnlockContext unlock;

    private List<IDialogueCommand> commands = new List<IDialogueCommand>();
    private Dictionary<string, int> labels = new Dictionary<string, int>();
    private int currentCommandIndex = 0;
    private bool isActive = false;


    public DialoguePlayer(DialogueUI ui, DialogueCommandFactory factory, UnlockContext unlock)
    {
        this.ui = ui;
        this.factory = factory;
        this.unlock = unlock;
    }

    public void LoadDialogue(TextAsset csvFile)
    {
        commands.Clear();
        labels.Clear();

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 헤더 스킵
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] data = ParseCSVLine(lines[i]); // 개선된 파싱

            if (data.Length == 0) continue;

            string commandType = data[0];
            string[] parameters = new string[data.Length - 1];
            Array.Copy(data, 1, parameters, 0, parameters.Length);

            // Label은 인덱스만 저장
            if (commandType == "Label")
            {
                labels[parameters[0]] = commands.Count;
                continue;
            }

            IDialogueCommand cmd = factory.CreateCommand(commandType, parameters);
            if (cmd != null)
                commands.Add(cmd);
        }
    }

    // CSV 파싱 헬퍼 (큰따옴표 처리)
    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField.Trim());
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        result.Add(currentField.Trim());
        return result.ToArray();
    }

    public void StartDialogue()
    {
        isActive = true;
        currentCommandIndex = 0;
        ui.Show();
        ExecuteNextCommand();
    }

    public void OnPlayerAdvance() // UI 클릭 시 호출
    {
        if (!isActive) return;
        ExecuteNextCommand();
    }

    private void ExecuteNextCommand()
    {
        if (currentCommandIndex >= commands.Count)
        {
            EndDialogue();
            return;
        }

        IDialogueCommand cmd = commands[currentCommandIndex];
        currentCommandIndex++;

        cmd.Execute(this);

        if (!isActive)
        {
            return;
        }

        // Blocking이 아니면 즉시 다음 명령어 실행
        if (!cmd.IsWaitingInput)
            ExecuteNextCommand();
    }


    public void EndDialogue()
    {
        isActive = false;
        ui.OnEndDialogue();
    }

    public void JumpToLabel(string labelName)
    {
        if (labels.ContainsKey(labelName))
        {
            currentCommandIndex = labels[labelName];
            ExecuteNextCommand();
        }
    }

    public void SelectChoice(string labelName)
    {
        JumpToLabel(labelName);
        ui.OnChoiceSelected();
    }
}
