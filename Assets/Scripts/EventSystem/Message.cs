using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string targetName;
    public string senderName;

    public ushort messageType;

    public Message(string target, string sender, ushort type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;
    }
}