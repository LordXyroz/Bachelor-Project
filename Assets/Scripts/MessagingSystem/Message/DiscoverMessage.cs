using System.Collections.Generic;

/// <summary>
/// Message sent when discovering new nodes.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class DiscoverMessage : Message
{
    public int depth;
    public float probability;
    public List<float> randValues;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="searchDepth">Search depth in network tree</param>
    /// <param name="prob">Probabilty of success</param>
    /// <param name="randomValues">Random value</param>
    public DiscoverMessage(string target, string sender, ushort type, int searchDepth, float prob, List<float> randomValues) :
        base(target, sender, type)
    {
        depth = searchDepth;
        probability = prob;
        randValues = randomValues;
    }
}
