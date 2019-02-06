using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
