/// <summary>
/// Message sent after probe with results.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class ProbeResponseMessage : Message
{
    public int numOfVulnerabilities;
    public int numOfChildren;
    public int difficulty;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="num">Number of child nodes</param>
    /// <param name="diff">Diffulty of the node</param>
    /// <param name="vuln">Number of vulnerabilities on the node</param>
    public ProbeResponseMessage(string target, string sender, ushort type, int num, int diff, int vuln)
        : base(target, sender, type)
    {
        numOfVulnerabilities = vuln;
        numOfChildren = num;
        difficulty = diff;
    }
}
