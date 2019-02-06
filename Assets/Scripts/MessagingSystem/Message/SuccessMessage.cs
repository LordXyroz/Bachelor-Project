using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing success/fail event data.
/// </summary>
public class SuccessMessage : Message
{
    public bool success;

    public SuccessMessage(string target, string sender, ushort type, bool response) 
        : base(target, sender, type)
    {
        success = response;
    }
}
