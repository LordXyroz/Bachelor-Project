using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyzeMessage : Message
{
    public float probability;

    public AnalyzeMessage(string target, string sender, ushort type, float prob) 
        : base(target, sender, type)
    {
        probability = prob;
    }
}
