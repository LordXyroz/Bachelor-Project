using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagingInterfaces;

/// TODO!!!!!!!
/// Maybe use coroutines for handling discover/analyze/attack??

/// <summary>
/// Handles the attacker's controls and capabilities.
/// </summary>
public class Attacker : MonoBehaviour, IDiscoverResponse, IAnalyzeResponse, IAttackResponse, IProbeResponse, IDefenseResponse
{ 
    [Header("Attack requirements")]
    public GameObject[] attackPrefabs;
    
    public GameObject target;

    [SerializeField]
    private List<NodeInfo> info = new List<NodeInfo>();

    [SerializeField]
    private AttackerUI uiScript;

    [Header("Discovery variables")]
    public int discoverLevel = 1;
    public float discoverProbability = 0.8f;
    [SerializeField]
    private int discoverUpgradeDuration = 10;
    private float discoverUpgradeTimer = 0f;
    private bool discoverUpgrading = false;
    private int discoverUpgradeCost = 10;

    private int discoverDuration = 3;
    private float discoverTimer = 0f;
    private bool discoverInProgress = false;
    private int discoverCost = 10;

    [Header("Analysis variables")]
    public int analyzeLevel = 1;
    public float analyzeProbability = 0.7f;
    [SerializeField]
    private int analyzeUpgradeDuration = 10;
    private float analyzeUpgradeTimer = 0f;
    private bool analyzeUpgrading = false;
    private int analyzeUpgradeCost = 10;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeInProgress = false;
    private int analyzeCost = 10;


    [Header("Probing variables")]
    private int probeDuration = 3;
    private float probeTimer = 0f;
    private bool probeInProgress = false;
    private int probeCost = 3;

    [Header("Attack variables")]
    public int attackLevel = 1;
    public float attackProbability = 0.6f;
    [SerializeField]
    private int attackUpgradeDuration = 10;
    private float attackUpgradeTimer = 0f;
    private bool attackUpgrading = false;
    private int attackUpgradeCost = 10;

    [Header("General variables")]
    public int resources = 100;
    private bool workInProgress = false;
    private NetworkingManager networking;

    /// <summary>
    /// Finds the networking manager.
    /// </summary>
    void Start()
    {
        networking = FindObjectOfType<NetworkingManager>();
    }

    /// <summary>
    /// Increments timers and calls functions once durations have been reached.
    /// </summary>
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

        uiScript.UpdateStats(resources, attackLevel, analyzeLevel, discoverLevel);
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
    /// Function to be hooked up with a button. Starts timer for Discovering.
    /// </summary>
    public void StartDiscover()
    {
        if (!workInProgress)
        {
            if (resources < discoverCost)
            {
                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to discover!");
                return;
            }

            if (uiScript.popupWindowObject.activeSelf)
                return;

            NodeInfo i = null;

            if (target != null)
            {
                i = info.Find(x => x.component.name == target.name);
                if (i != null)
                {
                    if (i.exploitable && !i.exploited)
                    {
                        uiScript.TogglePopupWindow(true, "Error", "Unable to discover on unexploited node!");
                        return;
                    }
                }
            }

            workInProgress = true;
            discoverInProgress = true;

            resources -= discoverCost;

            string msg = "Started discovering" + 
                ((target != null && i != null) ? " on: "  + i.displayName : "");

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

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
            if (resources < analyzeCost)
            {
                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to analyze!");
                return;
            }

            if (target == null)
                return;

            if (uiScript.popupWindowObject.activeSelf)
                return;

            workInProgress = true;
            analyzeInProgress = true;

            resources -= analyzeCost;

            string msg = "Started analyzing on: " + info.Find(x => x.component.name == target.name).displayName;

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

            uiScript.UpdateProgressbar(analyzeTimer, analyzeDuration);
            uiScript.ToggleProgressbar(true, "Analyze", "Analyzing " + info.Find(x => x.component.name == target.name).displayName);
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

            if (resources < go.GetComponent<Attack>().cost)
            {
                Destroy(go);
                workInProgress = false;

                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to attack!");
                uiScript.ToggleProgressbar(false, "", "");
                return;
            }

            go.GetComponent<Attack>().target = target;
            go.GetComponent<Attack>().probability = attackProbability;
            go.GetComponent<Attack>().targetDisplayName = info.Find(x => x.component.name == target.name).displayName;

            resources -= go.GetComponent<Attack>().cost;

            string msg = "Started attack of type: " + go.GetComponent<Attack>().attackType + ", on: " 
                + info.Find(x => x.component.name == target.name).displayName;

            networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts probing a target.
    /// </summary>
    public void StartProbing()
    {
        if (!workInProgress)
        {
            if (resources < probeCost)
            {
                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to probe!");
                return;
            }

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
    /// Sends a discover message.
    /// Resets timers/bools.
    /// </summary>
    void Discover()
    {
        discoverInProgress = false;
        discoverTimer = 0f;
        
        int currentDepth = -1;
        string targetName = "";
        List<float> randValues = new List<float>();

        if (target == null)
        {
            currentDepth = 0;
            randValues.Add(Random.Range(0f, 1f));
        }
        else
        {
            currentDepth = target.GetComponent<GameNetworkComponent>().graphDepth;
            targetName = target.GetComponent<GameNetworkComponent>().name;

            int count = info.Find(x => x.component.name == target.name).numOfChildren;

            if (count <= 0)
                count = 99;

            for (int i = 0; i < count; i++)
                randValues.Add(Random.Range(0f, 1f));
        }
        
        networking.SendMessage(new DiscoverMessage(targetName, name, MessageTypes.Game.Discover, currentDepth, discoverProbability, randValues));
    }

    /// <summary>
    /// Sends an analyze message.
    /// Rests timers/bools.
    /// </summary>
    void Analyze()
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
    void Probe()
    {
        probeInProgress = false;
        probeTimer = 0f;
        
        networking.SendMessage(new Message(target.name, name, MessageTypes.Game.Probe));
    }

    /// <summary>
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
            for (int j = 0; j < message.discovered.Count; j++)
            {
                msg += message.discovered[j] + ", ";
                info.Add(new NodeInfo(GameObject.Find(message.discovered[j])));
                NodeInfo n = info.Find(x => x.component.name == message.discovered[j]);
                n.exploitable = message.exploitables[j];
            }
            uiScript.TogglePopupWindow(true, "Success!", "New locations have been revealed!");
        }
        
        networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));
        
        uiScript.ToggleProgressbar(false, "", "");
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
    /// From the IAttackResponse interface.
    /// 
    /// Listens to a MessageTypes.Game.ATTACK_RESPONSE
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void AttackResponse(SuccessMessage message)
    {
        workInProgress = false;

        NodeInfo i = info.Find(x => x.component.name == message.senderName);
        if (i != null)
            if (message.success)
                i.exploited = true;

        string msg = "Attack response: " + ((message.success) ? "Success" : "Failed");

        networking.SendMessage(new LoggingMessage("", name, MessageTypes.Logging.Log, msg));

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
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void DefenseResponse(SuccessMessage message)
    {
        NodeInfo i = info.Find(x => x.component.name == message.senderName);
        if (i != null)
        {
            i.upToDate = false;
        }
    }

    /// <summary>
    /// Function for starting upgrade of attacks.
    /// Returns early if the level is at or above max level.
    /// </summary>
    public void StartAttackUpgrade()
    {
        if (attackLevel >= 3)
            return;
        
        if (!workInProgress)
        {
            if (resources < attackUpgradeCost)
            {
                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to upgrade attacks!");
                return;
            }

            workInProgress = true;
            attackUpgrading = true;

            resources -= attackUpgradeCost;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Attacks");
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
            if (resources < analyzeUpgradeCost)
            {
                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to upgrade analysis!");
                return;
            }

            workInProgress = true;
            analyzeUpgrading = true;

            resources -= analyzeUpgradeCost;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Analysis");
        }
    }

    /// <summary>
    /// Function for starting upgrade of discovery.
    /// Returns early if the level is at or above max level.
    /// </summary>
    public void StartDiscoverUpgrade()
    {
        if (discoverLevel >= 3)
            return;

        if (!workInProgress && resources >= discoverUpgradeCost)
        {
            if (resources < discoverUpgradeCost)
            {
                uiScript.TogglePopupWindow(true, "Error", "Lacking funds to upgrade discovery!");
                return;
            }

            workInProgress = true;
            discoverUpgrading = true;

            resources -= discoverUpgradeCost;

            uiScript.ToggleProgressbar(true, "Upgrade", "Upgrading Discovery");
        }
    }
    
    /// <summary>
    /// Handles the actual upgrade of attack.
    /// Increments level and increases probability.
    /// Resets timers and toggles.
    /// </summary>
    void UpgradeAttack()
    {
        attackLevel++;
        attackProbability += 0.32f;

        attackUpgrading = false;
        attackUpgradeDuration += 10;
        attackUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Attacks now upgraded to level: " + attackLevel);
        uiScript.UpdateStats(resources, attackLevel, analyzeLevel, discoverLevel);
        workInProgress = false;
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
        uiScript.UpdateStats(resources, attackLevel, analyzeLevel, discoverLevel);
        workInProgress = false;
    }

    /// <summary>
    /// Handles the actual upgrade of discovery.
    /// Increments level and increases probability.
    /// Resets timers and toggles.
    /// </summary>
    void UpgradeDiscover()
    {
        discoverLevel++;
        discoverProbability += 0.32f;

        discoverUpgrading = false;
        discoverUpgradeDuration += 10;
        discoverUpgradeTimer = 0f;

        uiScript.ToggleProgressbar(false, "", "");
        uiScript.TogglePopupWindow(true, "Success!", "Discovery now upgraded to level: " + discoverLevel);
        uiScript.UpdateStats(resources, attackLevel, analyzeLevel, discoverLevel);
        workInProgress = false;
    }


}
