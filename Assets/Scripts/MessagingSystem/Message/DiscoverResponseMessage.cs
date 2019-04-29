using System.Collections.Generic;

/// <summary>
/// Message sent after discover with results.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class DiscoverResponseMessage : Message
{
    public List<string> discovered;
    public string displayName;
    public List<bool> exploitables;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="response">List of names of gameobjects (nodes) found</param>
    /// <param name="dispName">Display name of node</param>
    /// <param name="exploits">List of if found nodes are exploitable</param>
    public DiscoverResponseMessage(string target, string sender, ushort type, List<string> response, string dispName, List<bool> exploits)
        : base(target, sender, type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;
    
        discovered = response;
        displayName = dispName;
        exploitables = exploits;
    }
}
