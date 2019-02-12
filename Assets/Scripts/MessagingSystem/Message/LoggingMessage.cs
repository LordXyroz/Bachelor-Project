using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing msg for logging events.
/// </summary>
public class LoggingMessage : Message
{
    public string message;

    public LoggingMessage(string target, string sender, ushort type, string msg) 
        : base(target, sender, type)
    {
        message = msg;
    }
}
