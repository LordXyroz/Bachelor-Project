[System.Serializable]
public class DiscClientMessage : Message
{
    public string disconnectingClient;

    public DiscClientMessage(string target, string sender, ushort type, string client)
        : base(target, sender, type)
    {
        disconnectingClient = client;
    }
}
