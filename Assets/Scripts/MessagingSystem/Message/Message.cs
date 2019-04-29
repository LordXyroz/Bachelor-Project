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
    /// Default constructor
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    public Message(string target, string sender, ushort type)
    {
        targetName = target;
        senderName = sender;
        messageType = type;

        playerType = Object.FindObjectOfType<NetworkingManager>().playerType;
    }
}