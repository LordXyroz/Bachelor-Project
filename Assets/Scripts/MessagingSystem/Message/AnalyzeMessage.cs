using System.Collections.Generic;

/// <summary>
/// Analysis messege sent to find vulnerabilties on a node.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class AnalyzeMessage : Message
{
    public float probability;
    public List<float> randValues;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="prob">Probability of success</param>
    /// <param name="rand">List of random values</param>
    public AnalyzeMessage(string target, string sender, ushort type, float prob, List<float> rand) 
        : base(target, sender, type)
    {
        probability = prob;
        randValues = rand;
    }
}
