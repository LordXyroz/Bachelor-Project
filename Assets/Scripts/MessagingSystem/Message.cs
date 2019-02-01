using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string targetName;
    public string senderName;

    public ushort messageType;

    public AttackTypes attack;
    public DefenseTypes defense;

    public bool success;

    public Message(string target, string sender, ushort type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackTypes.zero;
        defense = DefenseTypes.zero;

        success = false;
    }

    public Message(string target, string sender, ushort type, bool successful)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackTypes.zero;
        defense = DefenseTypes.zero;

        success = successful;
    }

    public Message(string target, string sender, ushort type, AttackTypes attackEnum)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = attackEnum;
        defense = DefenseTypes.zero;

        success = false;
    }

    public Message(string target, string sender, ushort type, DefenseTypes defenseEnum)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackTypes.zero;
        defense = defenseEnum;

        success = false;
    }
}