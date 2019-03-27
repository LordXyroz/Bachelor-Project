using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing relevant info for the observer about a network node.
/// Contains more info than NodeInfo.
/// </summary>
[System.Serializable]
public class ObserverNodeInfo : NodeInfo
{
    public List<DefenseTypes> availableDefenses;
    public List<DefenseTypes> implementedDefenses;

    /// <summary>
    /// Default constructor to init lists
    /// </summary>
    /// <param name="target">The node that's been discovered</param>
    public ObserverNodeInfo(GameObject go, string dispName) 
        : base(go, dispName)
    {
        availableDefenses = new List<DefenseTypes>();
        implementedDefenses = new List<DefenseTypes>();
    }
}
