using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MessagingInterfaces;

/// <summary>
/// The class used for creating components for the ingame network.
/// Implements the IUnderAttack and IAddDefense interfaces to listen to messages.
/// Add this to an empty GameObject and give it a name to create a new component.
/// Tweak the lists in the Inspector tab in the Unity Editor to customize vulnerabilities,
/// and available defenses that can be bought on to it.
/// </summary>
public class GameNetworkComponent : MonoBehaviour, IUnderAttack, IAddDefense, IDiscover, IAnalyze, IProbe, ITargeting
{
    [Header("Lists")]
    public List<AttackTypes> vulnerabilities;
    public List<DefenseTypes> availableDefenses;
    public List<DefenseTypes> implementedDefenses;

    [Header("Variables")]
    [Range(1f, 5f)]
    public int difficulty = 1;
    private NetworkingManager networking;
    public bool rootNode = false;
    public string displayName;

    [SerializeField]
    private GameObject uiElement;
    [SerializeField]
    private Button uiButton;
    [SerializeField]
    private BaseUI uiScript;

    public GameObject selectionBox;
    public TMPro.TextMeshProUGUI nameText;

    [Header("Network")]
    public int graphDepth;
    public List<GameNetworkComponent> children = new List<GameNetworkComponent>();
    public List<GameObject> connectionLines = new List<GameObject>();

    [Header("Observer Related")]
    public GameObject attackerFound;
    private bool attackerSelected = false;
    private bool defenderSelected = false;
    
    /// <summary>
    /// Sets up the OnClick of the button attached to this object.
    /// </summary>
    public void Start()
    {
        uiScript = FindObjectOfType<PlayerManager>().GetUIScript();
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 pos = new Vector3(rectTransform.position.x + rectTransform.rect.height * 1.5f, rectTransform.position.y);

        if (FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Attacker)
        {
            uiButton.onClick.AddListener(uiScript.DisablePopupWindow);
            uiButton.onClick.AddListener(() => FindObjectOfType<Attacker>().SetTarget(gameObject));
            uiButton.gameObject.GetComponent<ClickHandler>().onRight = OnClick;

            uiElement.SetActive(false);
        }
        else if (FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Defender)
        {
            uiButton.onClick.AddListener(uiScript.DisablePopupWindow);
            uiButton.onClick.AddListener(() => FindObjectOfType<Defender>().SetTarget(gameObject));
            uiButton.gameObject.GetComponent<ClickHandler>().onRight = OnClick;
        }
        else if (FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
        {
            uiButton.onClick.AddListener(() => FindObjectOfType<Observer>().UpdateNode(gameObject));
        }

        networking = FindObjectOfType<NetworkingManager>();
    }

    /// <summary>
    /// Adds to the available defenses based on which vulnerability the component has. 
    /// Set during initialization of the scenario.
    /// </summary>
    public void InitComponent()
    {
        nameText.text = displayName;

        availableDefenses = new List<DefenseTypes>();
        foreach (var v in vulnerabilities)
            foreach (var d in VulnerabilityLogic.GetDefenses(v))
                if (!availableDefenses.Contains(d))
                    availableDefenses.Add(d);
    }

    /// <summary>
    /// Places the connection lines between this component and other connected nodes.
    /// Called during initialization of the scenario.
    /// </summary>
    /// <param name="connection">Script containing data for the connection</param>
    /// <param name="line">Line gameobject to be moved to correct location</param>
    public void InitConnectionLine(ConnectionReferences connection, GameObject line)
    {
        var lineToEnd = line.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
        var lineFromStart = line.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;

        var startPos = connection.referenceFromObject.transform.position;
        var endPos = connection.referenceToObject.transform.position;

        float XPosDiff = Mathf.Abs(startPos.x - endPos.x) / 100;        //100 pixels per unit
        float YPosDiff = Mathf.Abs(startPos.y - endPos.y) / 100;        //100 pixels per unit
        
        if (!connection.referenceFromObject.GetComponent<GameNetworkComponent>().connectionLines.Contains(line))
            connection.referenceFromObject.GetComponent<GameNetworkComponent>().connectionLines.Add(line);
        if (!connection.referenceToObject.GetComponent<GameNetworkComponent>().connectionLines.Contains(line))
            connection.referenceToObject.GetComponent<GameNetworkComponent>().connectionLines.Add(line);

        /// connection to the right
        if (startPos.x < endPos.x)
        {
            /// connection down
            if (startPos.y > endPos.y)
            {
                lineToEnd.localScale = new Vector3(XPosDiff, 0.05f, 1.0f);
                lineFromStart.localScale = new Vector3(0.05f, YPosDiff, 1.0f);
                line.transform.rotation = Quaternion.identity;

                lineFromStart.position = new Vector3(startPos.x, endPos.y + YPosDiff * 50);
                lineToEnd.position = new Vector3(lineFromStart.position.x + XPosDiff * 50, endPos.y);
            }
            ///connection up
            else
            {
                lineFromStart.localScale = new Vector3(0.05f, XPosDiff, 1.0f);
                lineToEnd.localScale = new Vector3(YPosDiff, 0.05f, 1.0f);
                line.transform.rotation = Quaternion.Euler(0, 0, 90);

                lineFromStart.position = new Vector3(startPos.x + XPosDiff * 50, startPos.y);
                lineToEnd.position = new Vector3(lineFromStart.position.x + XPosDiff * 50, endPos.y - YPosDiff * 50);
            }
        }
        /// connection to the left
        else
        {

            /// connection down
            if (startPos.y > endPos.y)
            {
                lineFromStart.localScale = new Vector3(0.05f, XPosDiff, 1.0f);
                lineToEnd.localScale = new Vector3(YPosDiff, 0.05f, 1.0f);
                line.transform.rotation = Quaternion.Euler(0, 0, 270);

                lineFromStart.position = new Vector3(startPos.x - XPosDiff * 50, startPos.y);
                lineToEnd.position = new Vector3(lineFromStart.position.x - XPosDiff * 50, endPos.y + YPosDiff * 50);
            }
            ///connection up
            else
            {
                lineFromStart.localScale = new Vector3(XPosDiff, 0.05f, 1.0f);
                lineToEnd.localScale = new Vector3(0.05f, YPosDiff, 1.0f);
                line.transform.rotation = Quaternion.Euler(0, 0, -180);

                lineFromStart.position = new Vector3(startPos.x - XPosDiff * 50, startPos.y);
                lineToEnd.position = new Vector3(lineFromStart.position.x - XPosDiff * 50, endPos.y - YPosDiff * 50);
            }
        }
    }

    /// <summary>
    /// OnClick function to be called by a ClickHandler script.
    /// Opens up the OnClickMenu next to this gameobject.
    /// </summary>
    public void OnClick()
    {
        PlayerManager.PlayerType player = FindObjectOfType<PlayerManager>().GetPlayerType();

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 pos = new Vector3(rectTransform.position.x + rectTransform.rect.height * 1.5f, rectTransform.position.y);

        if (player == PlayerManager.PlayerType.Attacker)
        {
            if (FindObjectOfType<Attacker>().target == gameObject)
            {
                uiScript.ToggleOnClickMenu(true, pos);
            }
        }
        else if (player == PlayerManager.PlayerType.Defender)
        {
            if (FindObjectOfType<Defender>().target == gameObject)
            {
                uiScript.ToggleOnClickMenu(true, pos);
            }
        }
    }

    /// <summary>
    /// From the IUnderAttack interface.
    ///
    /// Listens to a MessageTypes.Events.ATTACK.
    /// Checks if self is the target of this event and if the AttackType is valid.
    /// Loops through and checks it self is vulnerable to the attack, then loops through
    /// implemented defenses (if any) if the attack can be stopped.
    /// Broadcasts a message regarding the success of the attack.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void UnderAttack(AttackMessage message)
    {
        if (name == message.targetName)
        {
            if (message.attack != AttackTypes.zero)
            {
                bool isVulnerable = false;
                foreach (var vuln in vulnerabilities)
                {
                    if (message.attack == vuln)
                    {
                        isVulnerable = true;
                        foreach (var def in implementedDefenses)
                        {
                            if (VulnerabilityLogic.IsStoppedBy(message.attack, def))
                                isVulnerable = false;
                        }
                      
                        if (!(message.randVal <= message.probability - (0.8f * (difficulty / 5f - 0.2f))))
                            isVulnerable = false;
                    }
                }

                if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                    return;

                networking.SendMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.AttackResponse, isVulnerable));
            }
        }
    }

    /// <summary>
    /// From the IAddDefense interface.
    /// 
    /// Listens to a MessageTypes.Events.DEFENSE.
    /// Checks if self is the target of this event and if the DefendType is valid.
    /// Removes the defense from availableDefenses list if possible, and add it to the
    /// implementedDefenses list.
    /// Broadcasts message regarding the success of adding the defense.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void AddDefense(DefenseMessage message)
    {
        if (name == message.targetName)
        {
            if (message.defense != DefenseTypes.zero)
            {
                if (availableDefenses.Remove(message.defense) && message.randVal <= message.probability - (0.8f * (difficulty / 5f - 0.2f)))
                {
                    implementedDefenses.Add(message.defense);

                    if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                        return;

                    networking.SendMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.DefenseResponse, true));
                }
                else
                {
                    if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                        return;

                    networking.SendMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.DefenseResponse, false));
                }
            }
        }
    }

    /// <summary>
    /// From the IDiscover interface.
    /// 
    /// Listens to a MessageTypes.Events.DISCOVER.
    /// Makes itself visible on the UI and returns a response.
    /// TODO: Implement linked list/tree structure for network components, and only reply/turn 
    /// visible when the correct depth has been reached in the tree.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void OnDiscover(DiscoverMessage message)
    {
        List<string> list = new List<string>();
        List<bool> exploits = new List<bool>();

        if (message.targetName == "" && rootNode)
        {
            if (!uiElement.activeSelf)
            {
                uiElement.SetActive(true);
                list.Add(name);
                exploits.Add(vulnerabilities.Count > 0);
            }

            if (FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                attackerFound.SetActive(true);

            if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                return;

            networking.SendMessage(new DiscoverResponseMessage(message.senderName, name, MessageTypes.Game.DiscoverResponse, list, displayName, exploits));
        }
        else if (message.targetName == name)
        {
            int count = 0;
            foreach (var child in children)
            {
                if (message.randValues[count] <= message.probability - (0.8f * (difficulty / 5f - 0.2f)))
                {
                    if (!child.uiElement.activeSelf)
                    {
                        list.Add(child.name);
                        exploits.Add(child.vulnerabilities.Count > 0);
                        child.uiElement.SetActive(true);
                    }

                    if (FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        child.attackerFound.SetActive(true);
                }
                count++;
            }

            if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                return;

            networking.SendMessage(new DiscoverResponseMessage(message.senderName, name, MessageTypes.Game.DiscoverResponse, list, displayName, exploits));

        }
    }

    /// <summary>
    /// From the IAnalyze interface
    /// 
    /// Listens to a MessageTypes.Events.ANALYZE.
    /// If self is target, loops through vulnerabilities and defenses.
    /// Sends a response containing the list of current (not defended) vulnerabilities
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void OnAnalyze(AnalyzeMessage message)
    {
        if (message.targetName == name)
        {
            List<AttackTypes> vulnList = new List<AttackTypes>();
            int count = 0;

            foreach (var vuln in vulnerabilities)
            {
                bool vulnerable = false;

                if (message.randValues[count] <= message.probability - (0.8f * (difficulty / 5f - 0.2f)))
                    vulnerable = true;

                foreach (var def in implementedDefenses)
                {
                    if (VulnerabilityLogic.IsStoppedBy(vuln, def))
                        vulnerable = false;
                }
                if (vulnerable)
                    vulnList.Add(vuln);

                count++;
            }

            if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                return;

            networking.SendMessage(new AnalyzeResponeMessage(message.senderName, name, MessageTypes.Game.AnalyzeResponse, vulnList));
        }

    }

    /// <summary>
    /// From the IProbe interface
    /// 
    /// Sends a message with relevant probing information.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void OnProbe(Message message)
    {
        if (message.targetName == name)
        {
            int i = 0;
            foreach (var entry in vulnerabilities)
            {
                bool stopped = false;
                foreach (var def in implementedDefenses)
                    if (VulnerabilityLogic.IsStoppedBy(entry, def))
                        stopped = true;

                if (!stopped)
                    i++;
            }

            if (message.playerType != FindObjectOfType<PlayerManager>().GetPlayerType())
                return;

            networking.SendMessage(new ProbeResponseMessage(message.senderName, name, MessageTypes.Game.ProbeResponse, children.Count, difficulty, i));
        }
    }

    /// <summary>
    /// Observer only feature.
    /// Colors the button based on who targets it.
    /// </summary>
    /// <param name="message"></param>
    public void OnTargeting(Message message)
    {
        if (FindObjectOfType<PlayerManager>().GetPlayerType() != PlayerManager.PlayerType.Observer)
            return;

        if (message.targetName != name)
            return;

        var colors = uiButton.colors;

        if (message.senderName == "Attacker")
        {
            colors.normalColor = Color.red;
            attackerSelected = true;
        }
        else if (message.senderName == "Defender")
        {
            colors.normalColor = Color.blue;
            defenderSelected = true;
        }
        else if (message.senderName == "")
        {
            if (message.playerType == PlayerManager.PlayerType.Attacker && defenderSelected)
            {
                colors.normalColor = Color.blue;
                attackerSelected = false;
            }
            else if (message.playerType == PlayerManager.PlayerType.Defender && attackerSelected)
            {
                defenderSelected = false;
                colors.normalColor = Color.red;
            }
            else
            {
                colors.normalColor = Color.white;
                attackerSelected = false;
                defenderSelected = false;
            }
        }

        uiButton.colors = colors;
    }
    
}
