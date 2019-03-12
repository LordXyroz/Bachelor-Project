using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    [Header("Saved system component data")]
    public List<string> systemComponentNamesList = new List<string>();
    public List<Vector3> systemComponentPositionsList = new List<Vector3>();
    public List<string> systemComponentTypesList = new List<string>();
    public List<int> systemComponentSecurityLevelsList = new List<int>();
    public List<VulnerabilityWrapper> systemComponentVulnerabilyWrappersList = new List<VulnerabilityWrapper>();

    [Header("Saved reference line data")]
    public List<Vector3> connectionLinePosition = new List<Vector3>();
    public List<GameObject> referenceFromObject = new List<GameObject>();
    public List<GameObject> referenceToObject = new List<GameObject>();
    public List<string> referenceFromObjectName = new List<string>();
    public List<string> referenceToObjectName = new List<string>();
    public List<bool> hasFirewall = new List<bool>();

    [Header("Empty objects intended for later development cycles")]
    public List<string> OSList = new List<string>();
    public List<string> subnetList = new List<string>();
    public List<GameObject> configPresentList = new List<GameObject>();
    public List<string> keypairList = new List<string>();
    public List<bool> floatingIPList = new List<bool>();
    public List<User> usersList = new List<User>();

    [Header("The stats for attacker and defender")]
    public int attackerAttackLevel;
    public int attackerDiscoveryLevel;
    public int attackerAnalysisLevel;
    public int attackerResources;

    public int defenderDefenceLevel;
    public int defenderDiscoveryLevel;
    public int defenderResources;
}


/// <summary>
/// Need to make separate wrappers for nested list, as they are not supported by JsonUtility.ToJson
/// </summary>
[System.Serializable]
public class VulnerabilityWrapper
{
    public List<string> vulnerabilityWrapperList = new List<string>();
}