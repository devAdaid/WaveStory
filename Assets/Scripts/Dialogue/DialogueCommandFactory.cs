using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DialogueCommandFactory
{
    private Dictionary<string, Type> commandTypes = new Dictionary<string, Type>();

    public DialogueCommandFactory()
    {
        AutoRegisterCommands();
    }

    // Reflection으로 모든 Command 자동 등록
    private void AutoRegisterCommands()
    {
        // 현재 어셈블리의 모든 타입 검색
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            // IDialogueCommand 인터페이스를 구현하고 추상 클래스가 아닌 것만
            if (typeof(IDialogueCommand).IsAssignableFrom(type) &&
                !type.IsAbstract &&
                !type.IsInterface)
            {
                // DialogueCommand Attribute 찾기
                var attribute = type.GetCustomAttribute<DialogueCommandAttribute>();

                if (attribute != null)
                {
                    commandTypes[attribute.CommandName] = type;
                    //Debug.Log($"[DialogueCommandFactory] Registered: {attribute.CommandName} -> {type.Name}");
                }
            }
        }

        Debug.Log($"[DialogueCommandFactory] Total registered commands: {commandTypes.Count}");
    }

    public IDialogueCommand CreateCommand(string commandName, string[] parameters)
    {
        if (!commandTypes.ContainsKey(commandName))
        {
            Debug.LogWarning($"[DialogueCommandFactory] Unknown command: {commandName}");
            return null;
        }

        try
        {
            // 인스턴스 생성
            Type commandType = commandTypes[commandName];
            IDialogueCommand command = (IDialogueCommand)Activator.CreateInstance(commandType);

            // 파라미터 초기화
            command.Initialize(parameters);

            return command;
        }
        catch (Exception e)
        {
            Debug.LogError($"[DialogueCommandFactory] Failed to create command '{commandName}': {e.Message}");
            return null;
        }
    }

    // 디버깅용: 등록된 모든 명령어 출력
    public void PrintRegisteredCommands()
    {
        Debug.Log("=== Registered Commands ===");
        foreach (var kvp in commandTypes)
        {
            Debug.Log($"  {kvp.Key} -> {kvp.Value.Name}");
        }
    }
}