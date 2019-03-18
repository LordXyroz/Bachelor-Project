
[System.Serializable]
public class ChatMessage : Message
{
    public string message;

    public ChatMessage(string target, string sender, ushort type, string msg)
        : base(target, sender, type)
    {
        message = msg;
    }
}
