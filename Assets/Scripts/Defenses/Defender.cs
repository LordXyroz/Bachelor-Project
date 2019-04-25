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
    
    public GameObject target;

    [SerializeField]
    private List<NodeInfo> info = new List<NodeInfo>();

    [SerializeField]
    private DefenderUI uiScript;

    [Header("Analysis variables")]
    public int analyzeLevel = 1;
    public float analyzeProbability = 0.6f;
    [SerializeField]
    private int analyzeUpgradeDuration = 10;
    private float analyzeUpgradeTimer = 0f;
    private bool analyzeUpgrading = false;
    private int analyzeUpgradeCost = 10;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeInProgress = false;
    private int analyzeCost = 10;

    [Header("Defense variables")]
    public int defenseLevel = 1;
    public float defenseProbability = 0.6f;
    [SerializeField]
    private int defenseUpgradeDuration = 10;
    private float defenseUpgradeTimer = 0f;
    private bool defenseUpgrading = false;
    private int defenseUpgradeCost = 10;

    [Header("Probing variables")]
    private int probeDuration = 3;
    private float probeTimer = 0f;
    private bool probeInProgress = false;
    private int probeCost = 3;

    [Header("General variables")]
    public int resources = 100;
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
        
        uiScript.UpdateStats(resources, defenseLevel, analyzeLevel);
    }

    /// <summary>
    /// Changes the target for the next attack/analyze event.
    /// </summary>
    /// <param name="go">Gameobject set to be new target</param>
    public void SetTarget(GameObject go)
    {
        if (!workInProgress)
        {
            if (target != null)
            {
                networking.SendMessage(new Message(target.name, "", MessageTypes.Logging.Targeting));
                target.GetComponent<GameNetworkComponent>().selectionBox.SetActive(false);
            }

            target = go;

            target.GetComponent<GameNetworkComponent>().selectionBox.SetActive(true);
            
            NodeInfo i = info.Find(x => x.component.name == go.name);
            if (i != null)
                uiScript.PopulateInfoPanel(i);
            
            networking.SendMessage(new Message(target.name, name, MessageTypes.Logging.Targeting));
        }
    }

    /// <summary>
    /// Removes current target selection.
    /// </summary>
    public void ClearTarget()
    {
        if (!workInProgress)
        {
            if (target != null)
            {
                networking.SendMessage(new Message(target.name, "", MessageTypes.Logging.Targeting));
                target.GetComponent<GameNetworkComponent>().selectionBox.SetActive(false);
            }
            target = null;
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts timer for Analyzing
    /// </summary>
    public void StartAnalyze()
    {
        if (!workInProgress && resources >= analyzeCost)
        {
            if (target == null)
                return;

            workInProgress = true;
            analyzeInProgress = true;

            resources -= analyzeCost;

            string msg = "Started analyzing on: " + target.name;

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration);
            uiScript.ToggleProgressbar(true, "Analyze", "Analyzing " + info.Find(x => x.component.name == target.name).displayName);
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

            if (resources < go.GetComponent<Defense>().cost)
            {
                Destroy(go);
                workInProgress = false;

                uiScript.ToggleProgressbar(false, "", "");
                return;
            }

            go.GetComponent<Defense>().target = target;
            go.GetComponent<Defense>().probability = defenseProbability;
            go.GetComponent<Defense>().targetDisplayName = info.Find(x => x.component.name == target.name).displayName;
            resources -= go.GetComponent<Defense>().cost;

            string msg = "Started implementing defense: " + go.GetComponent<Defense>().defenseType + ", on: " 
                + info.Find(x => x.component.name == target.name).displayName;

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts probing a target.
    /// </summary>
    public void StartProbe()
    {
        if (!workInProgress && resources >= probeCost)
        {
            if (target == null)
                return;

            if (uiScript.popupWindowObject.activeSelf)
                return;

            workInProgress = true;
            probeInProgress = true;

            resources -= probeCost;

            string msg = "Started probing: " + target.name;

            networking.SendMessage(new LoggingMessage(target.name, name, MessageTypes.Logging.Log, msg));
            uiScript.UpdateProgressbar(probeTimer, probeDuration);
            uiScript.ToggleProgressbar(true, "Probe", "Probing " + info.Find(x => x.component.name == target.name).displayName);
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

        List<float> randValues = new List<float>();

        int count = info.Find(x => x.component.name == target.name).numOfVulnerabilities;

        if (count <= 0)
            count = 99;

        for (int i = 0; i < count; i++)
            randValues.Add(Random.Range(0f, 1f));

        networking.SendMessage(new AnalyzeMessage(target.name, name, MessageTypes.Game.Analyze, analyzeProbability, randValues));
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
            uiScript.TogglePopupWindow(true, "Success!", "Info on " + i.displayName + " found!");
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

        if (!workInProgress && resources >= analyzeUpgradeCost)
        {
            workInProgress = true;
            analyzeUpgrading = true;

            resources -= analyzeUpgradeCost;

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

        if (!workInProgress && resources >= defenseUpgradeCost)
        {
            workInProgress = true;
            defenseUpgrading = true;

            resources -= defenseUpgradeCost;

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
        uiScript.UpdateStats(resources, defenseLevel, analyzeLevel);
        workInProgress = false;
    }
}
