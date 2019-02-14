using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing attack event data.
/// </summary>
public class AttackMessage : Message
{
    public AttackTypes attack;
    public float probability;

    public AttackMessage(string target, string sender, ushort type, AttackTypes attackType, float prob) 
        : base(target, sender, type)
    {
        attack = attackType;
        probability = prob;
    }
}
