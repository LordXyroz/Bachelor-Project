[System.Serializable]
public class SwapMessage : Message
{
    public ClientBehaviour.SwapMsgType swapMsg;

    public SwapMessage(string target, string sender, ushort type, ClientBehaviour.SwapMsgType swap)
        : base(target, sender, type)
    {
        swapMsg = swap;
    }
}
