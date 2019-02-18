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
    [SerializeField]
    private GameObject[] attackPrefabs;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private AttackerUI uiScript;

    private int discoverDuration = 3;
    private float discoverTimer = 0f;
    private bool discoverInProgress = false;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeInProgress = false;

    private int probeDuration = 3;
    private float probeTimer = 0f;
    private bool probeInProgress = false;

    private bool workInProgress = false;

    private float attackProbability = 0.4f;
    private float analyzeProbability = 0.6f;
    private float discoverProbability = 0.6f;

    // Update is called once per frame
    void Update()
    {
        if (discoverInProgress)
        {
            discoverTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(discoverTimer, discoverDuration, "Discover", "Discovering new locations..");
        }

        if (discoverTimer >= discoverDuration)
            Discover();

        if (analyzeInProgress)
        {
            analyzeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration, "Analyze", "Analyzing location...");
        }

        if (analyzeTimer >= analyzeDuration)
            Analyze();


        if (probeInProgress)
        {
            probeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(probeTimer, probeDuration, "Probe", "Probing " + target.name);
        }

        if (probeTimer >= probeDuration)
            Probe();
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartDiscover();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartAnalyze();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartProbing();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartAttack(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartAttack(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartAttack(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartAttack(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartAttack(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartAttack(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartAttack(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartAttack(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartAttack(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartAttack(9);
        }
    }

    /// <summary>
    /// Changes the target for the next attack/analyze event.
    /// </summary>
    /// <param name="go">Gameobject set to be new target</param>
    public void SetTarget(GameObject go)
    {
        if (!workInProgress)
            target = go;
    }

    /// <summary>
    /// Function to be hooked up with a button. Starts timer for Discovering.
    /// </summary>
    public void StartDiscover()
    {
        if (!workInProgress)
        {
            workInProgress = true;
            discoverInProgress = true;

            string msg = "Started discovering" + ((target != null) ? " on: "  + target.name : "");

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
            uiScript.UpdateProgressbar(discoverTimer, discoverDuration, "Discover", "Discovering new locations..");
            //uiScript.UpdateInfo(msg);
            uiScript.ToggleProgressbar(true);
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

            workInProgress = true;
            analyzeInProgress = true;

            string msg = "Started analyzing on: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration, "Analyze", "Analyzing " + target.name);
            //uiScript.UpdateInfo(msg);
            uiScript.ToggleProgressbar(true);
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

            workInProgress = true;

            var go = Instantiate(attackPrefabs[id]);
            go.GetComponent<Attack>().target = target;
            go.GetComponent<Attack>().probability = attackProbability;

            string msg = "Started attack of type: " + go.GetComponent<Attack>().attackType + ", on: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
            //uiScript.UpdateInfo(msg);
        }
    }


    public void StartProbing()
    {
        if (!workInProgress)
        {
            if (target == null)
                return;
            
            workInProgress = true;
            probeInProgress = true;

            string msg = "Started probing: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage(target.name, name, MessageTypes.Logging.LOG, msg));
            uiScript.UpdateProgressbar(probeTimer, probeDuration, "Probe", "Probing " + target.name);
            uiScript.ToggleProgressbar(true);
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

    public void Probe()
    {
        probeInProgress = false;
        probeTimer = 0f;

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

        string msg = "Discover response: ";

        if (message.discovered.Count == 0)
            msg += "nothing discovered";
        else
        {
            msg += "discovered - ";
            foreach (var entry in message.discovered)
                msg += entry.name + ", ";
        }

        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
        //uiScript.UpdateInfo(msg);
        uiScript.ToggleProgressbar(false);
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

            string msg = "Analyze response: ";

            if (message.attacks.Count == 0)
                msg += "no vulnerabilities found";
            else
            {
                msg += "found - ";
                foreach (var entry in message.attacks)
                    msg += entry + ", ";
            }

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
            //uiScript.UpdateInfo(msg);
            uiScript.ToggleProgressbar(false);
        }
    }

    /// <summary>
    /// From the IAttackResponse interface.
    /// 
    /// Listens to a MessageTypes.Events.ATTACK_RESPONSE
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void AttackResponse(SuccessMessage message)
    {
        workInProgress = false;

        string msg = "Attack response: " + ((message.success) ? "Success" : "Failed");

        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
        //uiScript.UpdateInfo(msg);
        uiScript.ToggleProgressbar(false);
    }

    public void OnProbeResponse(ProbeResponseMessage message)
    {
        workInProgress = false;

        string msg = "Probe response: devices - " + message.numOfChildren + ", difficulty - " + message.difficulty;
        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));

        uiScript.ToggleProgressbar(false);
    }
}
