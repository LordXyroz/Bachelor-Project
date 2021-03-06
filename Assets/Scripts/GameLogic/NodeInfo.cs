﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class containing relevant info for the players about a network node.
/// </summary>
[System.Serializable]
public class NodeInfo
{
    public GameObject component;
    public string displayName;

    public List<AttackTypes> vulnerabilities;

    public bool beenProbed;
    public bool beenAnalyzed;
    public bool beenDiscoveredOn;
    public bool upToDate;

    public bool exploited;
    public bool exploitable;

    public int numOfVulnerabilities;
    public int numOfChildren;
    public int difficulty;
    
    /// <summary>
    /// Default constructor that must be used.
    /// Sets up initial values for a newly discovered node.
    /// </summary>
    /// <param name="target">The node that's been discovered</param>
    public NodeInfo(GameObject target)
    {
        component = target;
        displayName = target.GetComponent<GameNetworkComponent>().displayName;

        vulnerabilities = new List<AttackTypes>();

        beenProbed = false;
        beenAnalyzed = false;
        beenDiscoveredOn = false;
        upToDate = true;

        exploited = false;

        numOfVulnerabilities = -1;
        numOfChildren = -1;
        difficulty = -1;
    }
}
