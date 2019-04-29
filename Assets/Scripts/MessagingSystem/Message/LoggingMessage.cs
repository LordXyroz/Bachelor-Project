/// <summary>
/// Message sent for logging gameplay info for sever.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class LoggingMessage : Message
{
    public string message;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="msg">String to log</param>
    public LoggingMessage(string target, string sender, ushort type, string msg) 
        : base(target, sender, type)
    {
        message = msg;
    }
}
