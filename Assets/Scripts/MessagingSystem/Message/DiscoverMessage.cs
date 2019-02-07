using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverMessage : Message
{
    public int depth;

    public DiscoverMessage(string target, string sender, ushort type, int searchDepth) :
        base(target, sender, type)
    {
        depth = searchDepth;
    }
}
