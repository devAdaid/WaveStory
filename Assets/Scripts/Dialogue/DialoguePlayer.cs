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
        if (csvFile == null)
        {
            Debug.LogError("[DialoguePlayer] CSV file is null!");
            return;
        }

        commands.Clear();
        labels.Clear();
        currentCommandIndex = 0;

        string[] lines = csvFile.text.Split('\n');

        // 헤더 검증
        if (lines.Length <= 1)
        {
            Debug.LogError("[DialoguePlayer] CSV file is empty or has no data rows!");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] data = ParseCSVLine(lines[i]);
            if (data.Length == 0) continue;

            string commandType = data[0].Trim();
            if (string.IsNullOrEmpty(commandType)) continue;

            string[] parameters = new string[data.Length - 1];
            for (int j = 0; j < parameters.Length; j++)
            {
                parameters[j] = data[j + 1].Trim();
            }

            // Label은 인덱스만 저장
            if (commandType == "Label")
            {
                if (parameters.Length > 0 && !string.IsNullOrEmpty(parameters[0]))
                {
                    if (labels.ContainsKey(parameters[0]))
                    {
                        Debug.LogWarning($"[DialoguePlayer] Duplicate label '{parameters[0]}' at line {i}");
                    }
                    labels[parameters[0]] = commands.Count;
                }
                continue;
            }

            IDialogueCommand cmd = factory.CreateCommand(commandType, parameters);
            if (cmd != null)
                commands.Add(cmd);
        }

        Debug.Log($"[DialoguePlayer] Loaded {commands.Count} commands and {labels.Count} labels");
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
        }
        else
        {
            Debug.LogError($"[DialoguePlayer] Label '{labelName}' not found!");
        }
    }

    public void SelectChoice(string labelName)
    {
        ui.OnChoiceSelected();
        JumpToLabel(labelName);
        ExecuteNextCommand(); // 선택 후에만 자동 실행
    }
}
