using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnalyzeMessage : Message
{
    public float probability;
    public List<float> randValues;

    public AnalyzeMessage(string target, string sender, ushort type, float prob, List<float> rand) 
        : base(target, sender, type)
    {
        probability = prob;
        randValues = rand;
    }
}
