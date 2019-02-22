using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the defender's controls and capabilities.
/// </summary>
public class Defender : MonoBehaviour, IAnalyzeResponse, IDefenseResponse
{
    [Header("Attack requirements")]
    public GameObject[] defensePrefabs;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private List<NodeInfo> info = new List<NodeInfo>();

    [SerializeField]
    private DefenderUI uiScript;

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

    [Header("General variables")]
    private bool workInProgress = false;

    // Update is called once per frame
    void Update()
    {
        if (analyzeInProgress)
            analyzeTimer += Time.deltaTime;

        if (analyzeTimer >= analyzeDuration)
            Analyze();

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartDefense(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartDefense(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartDefense(2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartDefense(3);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartDefense(4);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartDefense(5);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartDefense(6);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartDefense(7);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartDefense(8);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartDefense(9);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartDefense(10);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartDefense(11);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartDefense(12);
        }
    }

    /// <summary>
    /// Changes the target for the next attack/analyze event.
    /// </summary>
    /// <param name="go">Gameobject set to be new target</param>
    public void SetTarget(GameObject go)
    {
        target = go;
    }

    public void ClearTarget()
    {
        if (!workInProgress)
            target = null;
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts timer for Analyzing
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
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts a Defense implementation.
    /// </summary>
    /// <param name="id"></param>
    public void StartDefense(int id)
    {
        if (!workInProgress)
        {
            if (target == null)
                return;

            workInProgress = true;

            var go = Instantiate(defensePrefabs[id]);
            go.GetComponent<Defense>().target = target;

            string msg = "Started implementing defense: " + go.GetComponent<Defense>().defenseType + ", on: " + target.name;

            MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
        }
    }

    /// <summary>
    /// Sends an Analyze message.
    /// Resets timers/bools.
    /// </summary>
    public void Analyze()
    {
        analyzeInProgress = false;
        analyzeTimer = 0f;

        MessagingManager.BroadcastMessage(new Message(target.name, name, MessageTypes.Game.ANALYZE));
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
        }
    }
    /// <summary>
    /// From the IDefenseResponse interface.
    /// 
    /// Listens to a MessageTypes.Events.DEFENSE_RESPONSE
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void DefenseResponse(SuccessMessage message)
    {
        workInProgress = false;

        string msg = "Defense response: " + ((message.success) ? "Success" : "Failed");

        MessagingManager.BroadcastMessage(new LoggingMessage("", name, MessageTypes.Logging.LOG, msg));
    }
}
