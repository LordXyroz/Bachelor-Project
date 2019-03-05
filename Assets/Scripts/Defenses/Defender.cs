using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagingInterfaces;

/// <summary>
/// Handles the defender's controls and capabilities.
/// </summary>
public class Defender : MonoBehaviour, IAnalyzeResponse, IDefenseResponse, IProbeResponse
{
    [Header("Attack requirements")]
    public GameObject[] defensePrefabs;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private List<NodeInfo> info = new List<NodeInfo>();

    [SerializeField]
    private DefenderUI uiScript;

    [Header("Analysis variables")]
    [SerializeField]
    private int analyzeLevel = 1;
    [SerializeField]
    private float analyzeProbability = 0.6f;
    [SerializeField]
    private int analyzeUpgradeDuration = 10;
    private float analyzeUpgradeTimer = 0f;
    private bool analyzeUpgrading = false;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeInProgress = false;

    [Header("Defense variables")]
    [SerializeField]
    private int defenseLevel = 1;
    [SerializeField]
    private float defenseProbability = 0.6f;
    [SerializeField]
    private int defenseUpgradeDuration = 10;
    private float defenseUpgradeTimer = 0f;
    private bool defenseUpgrading = false;

    [Header("Probing variables")]
    private int probeDuration = 3;
    private float probeTimer = 0f;
    private bool probeInProgress = false;

    [Header("General variables")]
    [SerializeField]
    private bool workInProgress = false;
    private NetworkingManager networking;

    void Start()
    {
        foreach (var o in FindObjectsOfType<GameNetworkComponent>())
        {
            info.Add(new NodeInfo(o.gameObject));
        }
        networking = FindObjectOfType<NetworkingManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (analyzeUpgrading)
        {
            analyzeUpgradeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(analyzeUpgradeTimer, analyzeUpgradeDuration);
        }

        if (analyzeUpgradeTimer >= analyzeUpgradeDuration)
            UpgradeAnalyze();

        if (defenseUpgrading)
        {
            defenseUpgradeTimer += Time.deltaTime;
            uiScript.UpdateProgressbar(defenseUpgradeTimer, defenseUpgradeDuration);
        }

        if (defenseUpgradeTimer >= defenseUpgradeDuration)
            UpgradeDefense();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            target = GameObject.Find("GoogleAPI");
            StartDefense(0);
        }
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

    /// <summary>
    /// Removes current target selection.
    /// </summary>
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

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration);
            uiScript.ToggleProgressbar(true, "Analyze", "Analyzing " + target.name);
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
            go.GetComponent<Defense>().probability = defenseProbability;

            string msg = "Started implementing defense: " + go.GetComponent<Defense>().defenseType + ", on: " + target.name;

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts probing a target.
    /// </summary>
    public void StartProbe()
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

            networking.SendMessage(new LoggingMessage(target.name, name, MessageTypes.Logging.Log, msg));
            uiScript.UpdateProgressbar(probeTimer, probeDuration);
            uiScript.ToggleProgressbar(true, "Probe", "Probing " + target.name);
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

        networking.SendMessage(new AnalyzeMessage(target.name, name, MessageTypes.Game.Analyze, analyzeProbability));
    }

    /// <summary>
    /// Sends a probe message.
    /// Rests timers/bools.
    /// </summary>
    public void Probe()
    {
        probeInProgress = false;
        probeTimer = 0f;

        networking.SendMessage(new Message(target.name, name, MessageTypes.Game.Probe));
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
                
                if (!i.upToDate)
                    i.vulnerabilities = new List<AttackTypes>();

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

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

            uiScript.ToggleProgressbar(false, "", "");
            uiScript.PopulateInfoPanel(i);
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
        NodeInfo i = info.Find(x => x.component.name == message.senderName);
        if (i != null)
        {
            i.upToDate = false;
        }

        workInProgress = false;

        string msg = "Defense response: " + ((message.success) ? "Success" : "Failed");

        networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Defense", "Implementation was " + ((message.success) ? "successful" : "stopped"));
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
        if (message.targetName == name)
        {
            workInProgress = false;

            NodeInfo i = info.Find(x => x.component.name == message.senderName);
            i.beenProbed = true;
            i.numOfVulnerabilities = message.numOfVulnerabilities;
            i.numOfChildren = message.numOfChildren;
            i.difficulty = message.difficulty;

            string msg = "Probe response: devices - " + message.numOfChildren + ", difficulty - " + message.difficulty;
            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

            uiScript.ToggleProgressbar(false, "", "");
            uiScript.TogglePopupWindow(true, "Success!", "Info on " + message.senderName + " found!");
            uiScript.PopulateInfoPanel(i);
        }
    }

    /// <summary>
    /// Function for starting upgrade of analysis.
    /// Returns early if the level is at or above max level.
    /// </summary>
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

    /// <summary>
    /// Function for starting upgrade of defenses.
    /// Returns early if the level is at or above max level.
    /// </summary>
    public void StartDefenseUpgrade()
    {
        if (defenseLevel >= 3)
            return;

        if (!workInProgress)
        {
            workInProgress = true;
            defenseUpgrading = true;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Defenses");
        }
    }

    /// <summary>
    /// Handles the actual upgrade of analysis.
    /// Increments level and increases probability.
    /// Resets timers and toggles.
    /// </summary>
    void UpgradeAnalyze()
    {
        analyzeLevel++;
        analyzeProbability += 0.32f;

        analyzeUpgrading = false;
        analyzeUpgradeDuration += 10;
        analyzeUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Analysis now upgraded to level: " + analyzeLevel);
        uiScript.UpdateStats(0, defenseLevel, analyzeLevel);
        workInProgress = false;
    }

    /// <summary>
    /// Handles the actual upgrade of defenses.
    /// Increments level and increases probability.
    /// Resets timers and toggles.
    /// </summary>
    void UpgradeDefense()
    {
        defenseLevel++;
        defenseProbability += 0.32f;

        defenseUpgrading = false;
        defenseUpgradeDuration += 10;
        defenseUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Defenses now upgraded to level: " + defenseLevel);
        uiScript.UpdateStats(0, defenseLevel, analyzeLevel);
        workInProgress = false;
    }
}
