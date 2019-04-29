/// <summary>
/// Message sent after an attack or defense with success response.
/// Inherits from <see cref="Message"/> 
/// </summary>
[System.Serializable]
public class SuccessMessage : Message
{
    public bool success;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="response">Success response</param>
    public SuccessMessage(string target, string sender, ushort type, bool response) 
        : base(target, sender, type)
    {
        success = response;
    }
}
