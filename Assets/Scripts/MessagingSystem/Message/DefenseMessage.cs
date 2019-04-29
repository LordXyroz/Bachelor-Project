/// <summary>
/// Message sent when implementing a defense.
/// Inherits from <see cref="Message"/>
/// </summary>
[System.Serializable]
public class DefenseMessage : Message
{
    public DefenseTypes defense;
    public float probability;
    public float randVal;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="target">Target of the message</param>
    /// <param name="sender">Sender of the message</param>
    /// <param name="type">Message type</param>
    /// <param name="defenseEnum">Defense type</param>
    /// <param name="prob">Probabilty of success</param>
    /// <param name="rand">Random value</param>
    public DefenseMessage(string target, string sender, ushort type, DefenseTypes defenseEnum, float prob, float rand)
        : base(target, sender, type)
    {
        defense = defenseEnum;
        probability = prob;
        randVal = rand;
    }
}
