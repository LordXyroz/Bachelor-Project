using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string targetName;
    public string senderName;

    public ushort messageType;

    public AttackEnum attack;
    public DefenseEnum defense;

    public bool success;

    public Message(string target, string sender, ushort type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackEnum.zero;
        defense = DefenseEnum.zero;

        success = false;
    }

    public Message(string target, string sender, ushort type, bool successful)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackEnum.zero;
        defense = DefenseEnum.zero;

        success = successful;
    }

    public Message(string target, string sender, ushort type, AttackEnum attackEnum)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = attackEnum;
        defense = DefenseEnum.zero;

        success = false;
    }

    public Message(string target, string sender, ushort type, DefenseEnum defenseEnum)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackEnum.zero;
        defense = defenseEnum;

        success = false;
    }
}