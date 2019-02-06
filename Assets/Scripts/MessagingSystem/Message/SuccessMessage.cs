using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessMessage : Message
{
    public bool success;

    public SuccessMessage(string target, string sender, ushort type, bool response) 
        : base(target, sender, type)
    {
        success = response;
    }
}
