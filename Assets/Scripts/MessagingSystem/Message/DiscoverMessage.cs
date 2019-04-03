using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscoverMessage : Message
{
    public int depth;
    public float probability;
    public List<float> randValues;

    public DiscoverMessage(string target, string sender, ushort type, int searchDepth, float prob, List<float> randomValues) :
        base(target, sender, type)
    {
        depth = searchDepth;
        probability = prob;
        randValues = randomValues;
    }
}
