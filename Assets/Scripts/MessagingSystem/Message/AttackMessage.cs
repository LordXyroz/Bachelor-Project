using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing attack event data.
/// </summary>
[System.Serializable]
public class AttackMessage : Message
{
    public AttackTypes attack;
    public float probability;
    public float randVal;

    public AttackMessage(string target, string sender, ushort type, AttackTypes attackType, float prob, float rand) 
        : base(target, sender, type)
    {
        attack = attackType;
        probability = prob;
        randVal = rand;
    }
}
