using UnityEngine;

/// <summary>
/// Message sent to clients with the scenario to be played.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class SaveFileMessage : Message
{
    public string fileString;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="save">Scenario data <see cref="Save"/></param>
    public SaveFileMessage(string target, string sender, ushort type, Save save)
        : base(target, sender, type)
    {
        fileString = JsonUtility.ToJson(save);
    }
}
