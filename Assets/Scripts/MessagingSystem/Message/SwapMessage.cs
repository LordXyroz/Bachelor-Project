/// <summary>
/// Message sent when attacker of defender want to swap in lobby.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class SwapMessage : Message
{
    public ClientBehaviour.SwapMsgType swapMsg;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="swap">Swap details <see cref="ClientBehaviour.SwapMsgType"/></param>
    public SwapMessage(string target, string sender, ushort type, ClientBehaviour.SwapMsgType swap)
        : base(target, sender, type)
    {
        swapMsg = swap;
    }
}
