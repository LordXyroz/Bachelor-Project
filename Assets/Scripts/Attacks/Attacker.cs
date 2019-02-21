﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TODO!!!!!!!
/// Maybe use coroutines for handling discover/analyze/attack??

/// <summary>
/// Handles the attacker's controls and capabilities.
/// </summary>
public class Attacker : MonoBehaviour, IDiscoverResponse, IAnalyzeResponse, IAttackResponse, IProbeResponse
{ 
    public GameObject[] attackPrefabs;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private AttackerUI uiScript;

    [SerializeField]
    private List<NodeInfo> info = new List<NodeInfo>();

    [Header("Discovery variables")]
    [SerializeField]
    private int discoverLevel = 1;
    [SerializeField]
    private float discoverProbability = 0.8f;
    [SerializeField]
    private int discoverUpgradeDuration = 10;
    private float discoverUpgradeTimer = 0f;
    private bool discoverUpgrading = false;

    private int discoverDuration = 3;
    private float discoverTimer = 0f;
    private bool discoverInProgress = false;

    [Header("Analysis variables")]
    [SerializeField]
    private int analyzeLevel = 1;
    [SerializeField]
    private float analyzeProbability = 0.7f;
    [SerializeField]
    private int analyzeUpgradeDuration = 10;
    private float analyzeUpgradeTimer = 0f;
    private bool analyzeUpgrading = false;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeInProgress = false;

    [Header("Probing variables")]
    private int probeDuration = 3;
    private float probeTimer = 0f;
    private bool probeInProgress = false;

    [Header("Attack variables")]
    [SerializeField]
    private int attackLevel = 1;
    [SerializeField]
    private float attackProbability = 0.6f;
    [SerializeField]
    private int attackUpgradeDuration = 10;
    private float attackUpgradeTimer = 0f;
    private bool attackUpgrading = false;

    [Header("General variables")]
    private bool workInProgress = false;
    

    // Update is called once per frame
    void Update()
    {
        if (discoverInProgress)
        {
            discoverTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(discoverTimer, discoverDuration);
        }

        if (discoverTimer >= discoverDuration)
            Discover();

        if (analyzeInProgress)
        {
            analyzeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration);
        }

        if (analyzeTimer >= analyzeDuration)
            Analyze();


        if (probeInProgress)
        {
            probeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(probeTimer, probeDuration);
        }

        if (probeTimer >= probeDuration)
            Probe();

        if (attackUpgrading)
        {
            attackUpgradeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(attackUpgradeTimer, attackUpgradeDuration);
        }

        if (attackUpgradeTimer >= attackUpgradeDuration)
            UpgradeAttack();

        if (analyzeUpgrading)
        {
            analyzeUpgradeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(analyzeUpgradeTimer, analyzeUpgradeDuration);
        }

        if (analyzeUpgradeTimer >= analyzeUpgradeDuration)
            UpgradeAnalyze();

        if (discoverUpgrading)
        {
            discoverUpgradeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(discoverUpgradeTimer, discoverUpgradeDuration);
        }

        if (discoverUpgradeTimer >= discoverUpgradeDuration)
            UpgradeDiscover();
    }

    /// <summary>
    /// Changes the target for the next attack/analyze event.
    /// </summary>
    /// <param name="go">Gameobject set to be new target</param>
    public void SetTarget(GameObject go)
    {
        if (!workInProgress)
        {
            target = go;

            NodeInfo i = info.Find(x => x.component.name == go.name);
            if (i != null)
                uiScript.PopulateInfoPanel(i);
        }
    }

    public void ClearTarget()
    {
        if (!workInProgress)
            target = null;
    }

    /// <summary>
    /// Function to be hooked up with a button. Starts timer for Discovering.
    /// </summary>
    public void StartDiscover()
    {
        if (!workInProgress)
        {
            if (uiScript.popupWindowObject.activeSelf)
                return;

            workInProgress = true;
            discoverInProgress = true;

            string msg = "Started discovering" + ((target != null) ? " on: "  + target.name : "");

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));

            uiScript.UpdateProgressbar(discoverTimer, discoverDuration);
            uiScript.ToggleProgressbar(true, "Discover", "Discovering new locations..");
        }
    }

    /// <summary>
    /// Function to be hooked up with a button. Starts timer for Analyzing.
    /// </summary>
    public void StartAnalyze()
    {
        if (!workInProgress)
        {
            if (target == null)
                return;

            if (uiScript.popupWindowObject.activeSelf)
                return;

            workInProgress = true;
            analyzeInProgress = true;

            string msg = "Started analyzing on: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));

            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration);
            uiScript.ToggleProgressbar(true, "Analyze", "Analyzing " + target.name);
        }
    }

    /// <summary>
    /// Function to be hooked up with a button. Starts an Attack.
    /// </summary>
    /// <param name="id"></param>
    public void StartAttack(int id)
    {
        if (!workInProgress)
        {
            if (target == null)
                return;

            if (uiScript.popupWindowObject.activeSelf)
                return;

            workInProgress = true;

            var go = Instantiate(attackPrefabs[id]);
            go.GetComponent<Attack>().target = target;
            go.GetComponent<Attack>().probability = attackProbability;

            string msg = "Started attack of type: " + go.GetComponent<Attack>().attackType + ", on: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts probing a target.
    /// </summary>
    public void StartProbing()
    {
        if (!workInProgress)
        {
            if (target == null)
                return;

            if (uiScript.popupWindowObject.activeSelf)
                return;

            workInProgress = true;
            probeInProgress = true;

            string msg = "Started probing: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage(target.name, name, MessageTypes.Logging.LOG, msg));
            uiScript.UpdateProgressbar(probeTimer, probeDuration);
            uiScript.ToggleProgressbar(true, "Probe", "Probing " + target.name);
        }
    }

    /// <summary>
    /// Sends a discover message.
    /// Resets timers/bools.
    /// </summary>
    public void Discover()
    {
        discoverInProgress = false;
        discoverTimer = 0f;
        
        int currentDepth = -1;
        string targetName = "";
        if (target == null)
        {
            currentDepth = 0;
        }
        else
        {
            currentDepth = target.GetComponent<GameNetworkComponent>().graphDepth;
            targetName = target.GetComponent<GameNetworkComponent>().name;
        }
        
        MessagingManager.BroadcastMessage(new DiscoverMessage(targetName, name, MessageTypes.Game.DISCOVER, currentDepth, discoverProbability));
    }

    /// <summary>
    /// Sends an analyze message.
    /// Rests timers/bools.
    /// </summary>
    public void Analyze()
    {
        analyzeInProgress = false;
        analyzeTimer = 0f;
        
        MessagingManager.BroadcastMessage(new AnalyzeMessage(target.name, name, MessageTypes.Game.ANALYZE, analyzeProbability));
    }

    /// <summary>
    /// Sends a probe message.
    /// Rests timers/bools.
    /// </summary>
    public void Probe()
    {
        probeInProgress = false;
        probeTimer = 0f;
        Debug.Log("Probe triggered!");
        MessagingManager.BroadcastMessage(new Message(target.name, name, MessageTypes.Game.PROBE));
    }

    /// <summary>
    /// TODO: Implement function
    /// 
    /// From the IOnDiscoverResponse interface.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void OnDiscoverResponse(DiscoverResponseMessage message)
    {
        workInProgress = false;

        NodeInfo i = info.Find(x => x.component.name == message.senderName);


        if (i != null)
            i.beenDiscoveredOn = true;
        
        string msg = "Discover response: ";

        if (message.discovered.Count == 0)
        {
            msg += "nothing discovered";
            uiScript.TogglePopupWindow(true, "Failure", "No new locations found.");
        }
        else
        {
            msg += "discovered - ";
            foreach (var entry in message.discovered)
            {
                msg += entry.name + ", ";
                info.Add(new NodeInfo(entry.gameObject));
            }
            uiScript.TogglePopupWindow(true, "Success!", "New locations have been revealed!");
        }

        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
        
        uiScript.ToggleProgressbar(false, "", "");
        //uiScript.PopulateInfoPanel(info.Find(x => x.component.name == message.senderName));
    }

    /// <summary>
    /// From the IOnAnalyzeResponse interface.
    /// 
    /// Gets a list of vulnerabilities from the sender.
    /// TODO: Use the list for game logic.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void OnAnalyzeResponse(AnalyzeResponeMessage message)
    {
        if (message.targetName == name)
        {
            workInProgress = false;

            NodeInfo i = info.Find(x => x.component.name == message.senderName);
            i.beenAnalyzed = true;

            string msg = "Analyze response: ";

            if (message.attacks.Count == 0)
            {
                msg += "no vulnerabilities found";
                if (i.vulnerabilities.Count == 0)
                    uiScript.TogglePopupWindow(true, "Failure", "No vulnerabilties found");
                else
                    uiScript.TogglePopupWindow(true, "Failure", "No new vulnerabilties found");
            }
            else
            {
                msg += "found - ";
                bool foundNew = false;
                foreach (var entry in message.attacks)
                {
                    msg += entry + ", ";
                    if (!i.vulnerabilities.Contains(entry))
                    {
                        i.vulnerabilities.Add(entry);
                        foundNew = true;
                    }
                }
                if (foundNew)
                    uiScript.TogglePopupWindow(true, "Success!", "New vulnerabilities have been revealed!");
                else
                    uiScript.TogglePopupWindow(true, "Failure", "No new vulnerabilties found");
            }
            
            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
            
            uiScript.ToggleProgressbar(false, "", "");
            uiScript.PopulateInfoPanel(i);
        }
    }

    /// <summary>
    /// From the IAttackResponse interface.
    /// 
    /// Listens to a MessageTypes.Game.ATTACK_RESPONSE
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void AttackResponse(SuccessMessage message)
    {
        workInProgress = false;

        string msg = "Attack response: " + ((message.success) ? "Success" : "Failed");

        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Attack", "Attack was " + ((message.success) ? "successful" : "stopped"));
    }

    /// <summary>
    /// From the IProbeResponse interface.
    /// 
    /// Listens to a MessageTypes.Game.PROBE_RESPONSE.
    /// Updates the NodeInfo for the sender.
    /// </summary>
    /// <param name="message">Message containing the relevant info to be handled by the function</param>
    public void OnProbeResponse(ProbeResponseMessage message)
    {
        workInProgress = false;

        NodeInfo i = info.Find(x => x.component.name == message.senderName);
        i.beenProbed = true;
        i.numOfVulnerabilities = message.numOfVulnerabilities;
        i.numOfChildren = message.numOfChildren;
        i.difficulty = message.difficulty;

        string msg = "Probe response: devices - " + message.numOfChildren + ", difficulty - " + message.difficulty;
        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Info on " + message.senderName + " found!");
        uiScript.PopulateInfoPanel(i);
    }

    public void StartAttackUpgrade()
    {
        if (attackLevel >= 3)
            return;
        
        if (!workInProgress)
        {
            workInProgress = true;
            attackUpgrading = true;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Attacks");
        }
    }

    public void StartAnalyzeUpgrade()
    {
        if (analyzeLevel >= 3)
            return;

        if (!workInProgress)
        {
            workInProgress = true;
            analyzeUpgrading = true;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Analysis");
        }
    }
    
    public void StartDiscoverUpgrade()
    {
        if (discoverLevel >= 3)
            return;

        if (!workInProgress)
        {
            workInProgress = true;
            discoverUpgrading = true;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Discovery");
        }
    }
    
    public void UpgradeAttack()
    {
        attackLevel++;
        attackProbability += 0.32f;

        attackUpgrading = false;
        attackUpgradeDuration += 10;
        attackUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Attacks now upgraded to level: " + attackLevel);
        uiScript.UpdateStats(0, attackLevel, analyzeLevel, discoverLevel);
        workInProgress = false;
    }

    public void UpgradeDiscover()
    {
        discoverLevel++;
        discoverProbability += 0.32f;

        discoverUpgrading = false;
        discoverUpgradeDuration += 10;
        discoverUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Discovery now upgraded to level: " + discoverLevel);
        uiScript.UpdateStats(0, attackLevel, analyzeLevel, discoverLevel);
        workInProgress = false;
    }

    public void UpgradeAnalyze()
    {
        analyzeLevel++;
        analyzeProbability += 0.32f;

        analyzeUpgrading = false;
        analyzeUpgradeDuration += 10;
        analyzeUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Analysis now upgraded to level: " + analyzeLevel);
        uiScript.UpdateStats(0, attackLevel, analyzeLevel, discoverLevel);
        workInProgress = false;
    }
}
