using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing response data from a discover event.
/// </summary>
public class DiscoverResponseMessage : Message
{
    public bool success;

    public DiscoverResponseMessage(string target, string sender, ushort type, bool response)
        : base(target, sender, type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;
    
        success = response;
    }
}
