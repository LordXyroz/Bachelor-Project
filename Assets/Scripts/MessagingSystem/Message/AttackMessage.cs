using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMessage : Message
{
    public AttackTypes attack;

    public AttackMessage(string target, string sender, ushort type, AttackTypes attackType) 
        : base(target, sender, type)
    {
        attack = attackType;
    }
}
