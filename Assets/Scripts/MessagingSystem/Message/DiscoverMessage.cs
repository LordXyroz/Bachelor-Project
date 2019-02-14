using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverMessage : Message
{
    public int depth;
    public float probability;

    public DiscoverMessage(string target, string sender, ushort type, int searchDepth, float prob) :
        base(target, sender, type)
    {
        depth = searchDepth;
        probability = prob;
    }
}
