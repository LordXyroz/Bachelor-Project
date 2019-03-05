using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper class for storing data to be sent between GameObjects.
/// </summary>
[System.Serializable]
public class Message
{
    public string targetName;
    public string senderName;

    public ushort messageType;

    public PlayerManager.PlayerType playerType;

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

        playerType = Object.FindObjectOfType<PlayerManager>().GetPlayerType();
    }
}