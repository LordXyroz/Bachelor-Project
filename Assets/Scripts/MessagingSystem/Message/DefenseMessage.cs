using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseMessage : Message
{
    public DefenseTypes defense;

    public DefenseMessage(string target, string sender, ushort type, DefenseTypes defenseEnum)
        : base(target, sender, type)
    {
        defense = defenseEnum;
    }
}
