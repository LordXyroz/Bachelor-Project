using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing attack event data.
/// </summary>
public class AttackMessage : Message
{
    public AttackTypes attack;

    public AttackMessage(string target, string sender, ushort type, AttackTypes attackType) 
        : base(target, sender, type)
    {
        attack = attackType;
    }
}
