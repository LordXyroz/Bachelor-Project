using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player manager class that controls player type, game timer, and loads level data.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// Enum for all playertypes possible in the game
    /// </summary>
    public enum PlayerType
    {
        Observer,
        Attacker,
        Defender
    }

    [Header("Game and player")]
    [SerializeField]
    private PlayerType playerType;
    [SerializeField]
    private BaseUI uiScript;

    private float timeInSeconds = 0;
    private bool countDown = false;
    private bool counting = true;

    [Header("GameObjects")]
    public GameObject attackerUI;
    public GameObject defenderUI;
    public GameObject observerUI;
    public GameObject attacker;
    public GameObject defender;
    public GameObject observer;

    #region Scenario loading variables
    [Header("Prefabs")]
    public GameObject apiPrefab;
    public GameObject switchPrefab;
    public GameObject internetPrefab;
    public GameObject computerPrefab;
    public GameObject websitePrefab;
    public GameObject serverPrefab;
    public GameObject syscompPrefab;
    public GameObject connectionLinePrefab;

    [Header("Other")]
    private GameObject componentWindow;
    private readonly Vector3 offset = new Vector3(-164, -25, 0);
    #endregion

    /// <summary>
    /// Initializes values and enables gameobjects based on playertype
    /// </summary>
    void Awake()
    {
        playerType = FindObjectOfType<NetworkingManager>().playerType;

        if (playerType == PlayerType.Attacker)
        {
            attackerUI.SetActive(true);
            defenderUI.SetActive(false);
            observerUI.SetActive(false);

            attacker.SetActive(true);
            defender.SetActive(false);
            observer.SetActive(false);

            uiScript = FindObjectOfType<AttackerUI>();
        }
        else if (playerType == PlayerType.Defender)
        {
            attackerUI.SetActive(false);
            defenderUI.SetActive(true);
            observerUI.SetActive(false);

            attacker.SetActive(false);
            defender.SetActive(true);
            observer.SetActive(false);

            uiScript = FindObjectOfType<DefenderUI>();
        }
        else if (playerType == PlayerType.Observer)
        {
            attackerUI.SetActive(false);
            defenderUI.SetActive(false);
            observerUI.SetActive(true);

            attacker.SetActive(false);
            defender.SetActive(false);
            observer.SetActive(true);

            uiScript = FindObjectOfType<ObserverUI>();
        }

        componentWindow = GameObject.Find("GameplayElements");
        InitScenario();
    }

    /// <summary>
    /// Counts gametime, and ends the game when a limit has been reached.
    /// Counts up or down based on scenario info.
    /// </summary>
    public void Update()
    {
        if (counting)
        {
            if (countDown)
            {
                timeInSeconds -= Time.deltaTime;

                if (timeInSeconds <= 0)
                    counting = false;
            }
            else
            {
                timeInSeconds += Time.deltaTime;

                if (timeInSeconds >= 7200)
                    counting = false;
            }
        }
        else
        {
            uiScript.ToggleFinishScreen();
        }

        uiScript.UpdateTime(timeInSeconds);
    }

    /// <summary>
    /// Gets which uiscript is active for the player
    /// </summary>
    /// <returns>UI script which is active</returns>
    public BaseUI GetUIScript()
    {
        return uiScript;
    }
    
    /// <summary>
    /// Gets the player type currently playing.
    /// </summary>
    /// <returns>Playertpe currently playing</returns>
    public PlayerType GetPlayerType()
    {
        return playerType;
    }

    /// <summary>
    /// Init scenario function.
    /// Sets up everything necessary to run a scenario loaded from file.
    /// </summary>
    private void InitScenario()
    {
        /// Gets the secnario data, and inits basic stats.
        var scenarioData = FindObjectOfType<NetworkingManager>().saveFile;
        var compList = new List<GameNetworkComponent>();

        timeInSeconds = scenarioData.gameTimeInMinuttes * 60;
        countDown = (timeInSeconds > 0);

        /// Sets resources, level and probability on attacker and defender side.
        switch (playerType)
        {
            case PlayerType.Attacker:
                var attacker = FindObjectOfType<Attacker>();

                attacker.resources = scenarioData.attackerResources;
                attacker.analyzeLevel = scenarioData.attackerAnalysisLevel;
                attacker.attackLevel = scenarioData.attackerAttackLevel;
                attacker.discoverLevel = scenarioData.attackerDiscoveryLevel;

                attacker.analyzeProbability = 0.6f + ((attacker.analyzeLevel - 1) * 0.32f);
                attacker.attackProbability = 0.6f + ((attacker.attackLevel - 1) * 0.32f);
                attacker.discoverProbability = 0.8f + ((attacker.discoverLevel - 1) * 0.32f);

                break;
            case PlayerType.Defender:
                var defender = FindObjectOfType<Defender>();

                defender.resources = scenarioData.defenderResources;
                defender.analyzeLevel = scenarioData.defenderAnalysisLevel;
                defender.defenseLevel = scenarioData.defenderDefenceLevel;

                defender.analyzeProbability = 0.6f + ((defender.analyzeLevel - 1) * 0.32f);
                defender.defenseProbability = 0.6f + ((defender.defenseLevel - 1) * 0.32f);

                break;
        }

        /// Loops through and creates network nodes.
        for (int i = 0; i < scenarioData.systemComponentPositionsList.Count; i++)
        {
            VulnerabilityWrapper wrapper = scenarioData.systemComponentVulnerabilyWrappersList[i];

            Vector3 pos = scenarioData.systemComponentPositionsList[i] + offset;
            
            GameObject obj = null;
            switch (scenarioData.systemComponentTypesList[i])
            {
                case "API":
                    obj = Instantiate(apiPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                case "Internet":
                    obj = Instantiate(internetPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                case "Switch":
                    obj = Instantiate(switchPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                case "Website":
                    obj = Instantiate(websitePrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                case "Server":
                    obj = Instantiate(serverPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                case "Computer":
                    obj = Instantiate(computerPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                case "System component":
                    obj = Instantiate(syscompPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
                default:
                    Debug.LogError("Unkown component type. Is the scenario data malformed? Loading default");
                    obj = Instantiate(syscompPrefab, pos, Quaternion.identity, componentWindow.transform);
                    break;
            }

            obj.name += i;

            /// Sets up node variables.
            GameNetworkComponent component = obj.GetComponent<GameNetworkComponent>();

            component.difficulty = scenarioData.systemComponentSecurityLevelsList[i];
            component.vulnerabilities = wrapper.vulnerabilityWrapperList;
            component.rootNode = scenarioData.isEntryPointList[i];
            component.displayName = scenarioData.systemComponentNamesList[i];
            compList.Add(component);

            component.InitComponent();
        }

        /// Loops through every connection between nodes and creates a line.
        for (int i = 0; i < scenarioData.connectionLinePosition.Count; i++)
        {
            GameObject line = Instantiate(connectionLinePrefab, scenarioData.connectionLinePosition[i], Quaternion.identity, componentWindow.transform);
            line.transform.SetAsFirstSibling();

            /// Sets up which object the connection goes to and from.
            ConnectionReferences reference = line.GetComponent<ConnectionReferences>();
            reference.referenceFromObject = GameObject.Find(scenarioData.referenceFromObjectName[i]);
            reference.referenceToObject = GameObject.Find(scenarioData.referenceToObjectName[i]);

            /// Adds the to-objcet as child to the from-object in the network.
            reference.referenceFromObject.GetComponent<GameNetworkComponent>()
                    .children
                    .Add(reference.referenceToObject.GetComponent<GameNetworkComponent>());

            /// Adds the from-objcet as child to the to-object in the network.
            reference.referenceToObject.GetComponent<GameNetworkComponent>()
                    .children
                    .Add(reference.referenceFromObject.GetComponent<GameNetworkComponent>());

            /// Sets up firewall and position
            reference.hasFirewall = scenarioData.hasFirewall[i];
            reference.transform.position = scenarioData.firewallPositionsList[i];

            /// Moves and changes the line to connect between the nodes.
            reference.referenceFromObject
                .GetComponent<GameNetworkComponent>()
                .InitConnectionLine(reference, line);

            /// Enables the firewall if the connection has one
            if (reference.hasFirewall)
            {
                var firewall = line.gameObject.transform.Find("Firewall").gameObject;
                firewall.SetActive(true);
                var lineToEnd = line.gameObject.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
                var lineFromStart = line.gameObject.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;

                if (line.GetComponent<ConnectionReferences>().hasFirewall
                    && lineToEnd != null)
                {
                    /// Place the firewall icon on the longest vector/line
                    if (lineToEnd.localScale.magnitude > lineFromStart.localScale.magnitude)
                        firewall.transform.position = lineToEnd.position;
                    else
                        firewall.transform.position = lineFromStart.position;

                    /// Need to rotate the firewall if both are negative
                    if (firewall.transform.position.x < 0 && firewall.transform.position.y < 0)
                        firewall.transform.rotation = new Quaternion(0, 0, 90, 0);
                    else
                        firewall.transform.rotation = Quaternion.identity;

                    firewall.SetActive(true);
                }
                else if (firewall != null)
                {
                    firewall.SetActive(false);
                }
            }
        }
    }
}
