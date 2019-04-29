/// <summary>
/// Message sent when an attack happens.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class AttackMessage : Message
{
    public AttackTypes attack;
    public float probability;
    public float randVal;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="attackType">Attack type that is being sent</param>
    /// <param name="prob">Probability of success</param>
    /// <param name="rand">Random value</param>
    public AttackMessage(string target, string sender, ushort type, AttackTypes attackType, float prob, float rand) 
        : base(target, sender, type)
    {
        attack = attackType;
        probability = prob;
        randVal = rand;
    }
}
