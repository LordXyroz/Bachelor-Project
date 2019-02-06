using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing response data from an analyze event.
/// </summary>
public class AnalyzeResponeMessage : Message
{
    public List<AttackTypes> attacks;

    public AnalyzeResponeMessage(string target, string sender, ushort type, List<AttackTypes> attackTypes)
        : base(target, sender, type)
    {
        attacks = attackTypes;
    }
}
