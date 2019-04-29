using System.Collections.Generic;

/// <summary>
/// Message sent after analysis with the result.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class AnalyzeResponeMessage : Message
{
    public List<AttackTypes> attacks;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="attackTypes">List of found attacks</param>
    public AnalyzeResponeMessage(string target, string sender, ushort type, List<AttackTypes> attackTypes)
        : base(target, sender, type)
    {
        attacks = attackTypes;
    }
}
