/// <summary>
/// Message sent between server and client when client wants to connect to lobby.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class ConnectMessage : Message
{
    public string hostName;
    public string attackerName;
    public string defenderName;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="host">Name of server host</param>
    /// <param name="attacker">Name of connected attacker</param>
    /// <param name="defender">Name of connected defender</param>
    public ConnectMessage(string target, string sender, ushort type, string host, string attacker, string defender) :
        base(target, sender, type)
    {
        hostName = host;
        attackerName = attacker;
        defenderName = defender;
    }
}
