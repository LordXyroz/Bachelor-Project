using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AttackerInfo
{
    public GameObject component;

    public List<AttackTypes> vulnerabilities;

    public bool beenProbed;
    public bool beenAnalyzed;
    public bool beenDiscoveredOn;

    public int numOfVulnerabilities;
    public int numOfChildren;
    public int difficulty;

    public AttackerInfo(GameObject target = null)
    {
        component = target;
        
        vulnerabilities = new List<AttackTypes>();

        beenProbed = false;
        beenAnalyzed = false;
        beenDiscoveredOn = false;

        numOfVulnerabilities = 0;
        numOfChildren = 0;
        difficulty = 0;
    }
}
