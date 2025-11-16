using System;

[AttributeUsage(AttributeTargets.Class)]
public class DialogueCommandAttribute : Attribute
{
    public string CommandName { get; }

    public DialogueCommandAttribute(string commandName)
    {
        CommandName = commandName;
    }
}