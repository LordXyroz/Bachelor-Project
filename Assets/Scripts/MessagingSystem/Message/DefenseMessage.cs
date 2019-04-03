using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class of Message for storing defense event data.
/// </summary>
[System.Serializable]
public class DefenseMessage : Message
{
    public DefenseTypes defense;
    public float probability;
    public float randVal;

    public DefenseMessage(string target, string sender, ushort type, DefenseTypes defenseEnum, float prob, float rand)
        : base(target, sender, type)
    {
        defense = defenseEnum;
        probability = prob;
        randVal = rand;
    }
}
