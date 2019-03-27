using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing response data from a discover event.
/// </summary>
[System.Serializable]
public class DiscoverResponseMessage : Message
{
    public List<string> discovered;
    public string displayName;

    public DiscoverResponseMessage(string target, string sender, ushort type, List<string> response, string dispName)
        : base(target, sender, type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;
    
        discovered = response;
        displayName = dispName;
    }
}
