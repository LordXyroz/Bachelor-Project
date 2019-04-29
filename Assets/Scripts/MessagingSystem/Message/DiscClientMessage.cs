/// <summary>
/// Message sent when a client disconnects from the server.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class DiscClientMessage : Message
{
    public string disconnectingClient;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="client">Name of the client</param>
    public DiscClientMessage(string target, string sender, ushort type, string client)
        : base(target, sender, type)
    {
        disconnectingClient = client;
    }
}
