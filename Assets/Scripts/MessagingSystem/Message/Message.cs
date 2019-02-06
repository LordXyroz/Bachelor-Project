using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper class for storing data to be sent between GameObjects.
/// </summary>
public class Message
{
    public string targetName;
    public string senderName;

    public ushort messageType;

    public AttackTypes attack;
    public DefenseTypes defense;

    public bool success;

    /// <summary>
    /// Constructor when the message isn't an attack, defense, or response
    /// </summary>
    /// <param name="target">Name of the target GameObject</param>
    /// <param name="sender">name of the GameObject that sent the message</param>
    /// <param name="type">Type of message sent</param>
    public Message(string target, string sender, ushort type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackTypes.zero;
        defense = DefenseTypes.zero;

        success = false;
    }

    /// <summary>
    /// Overloaded constructor when the message is a response
    /// </summary>
    /// <param name="target">Name of the target GameObject</param>
    /// <param name="sender">name of the GameObject that sent the message</param>
    /// <param name="type">Type of message sent</param>
    /// <param name="successful">Whether the event succeeded</param>
    public Message(string target, string sender, ushort type, bool successful)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = AttackTypes.zero;
        defense = DefenseTypes.zero;

        success = successful;
    }

    /// <summary>
    /// Overloaded constructor when the message is an attack
    /// </summary>
    /// <param name="target">Name of the target GameObject</param>
    /// <param name="sender">name of the GameObject that sent the message</param>
    /// <param name="type">Type of message sent</param>
    /// <param name="attackEnum">What type of attack is sent</param>
    public Message(string target, string sender, ushort type, AttackTypes attackEnum)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        attack = attackEnum;
        defense = DefenseTypes.zero;

        success = false;
    }

    /// <summary>
    /// Overloaded constructor when the message is a defense
    /// </summary>
    /// <param name="target">Name of the target GameObject</param>
    /// <param name="sender">name of the GameObject that sent the message</param>
    /// <param name="type">Type of message sent</param>
    /// <param name="defenseEnum">What type of defense is sent</param>
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