/// <summary>
/// Message sent when a chat message is sent.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class ChatMessage : Message
{
    public string message;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="msg">Chat string sent</param>
    public ChatMessage(string target, string sender, ushort type, string msg)
        : base(target, sender, type)
    {
        message = msg;
    }
}
