using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class used for creating components for the ingame network.
/// Implements the IUnderAttack and IAddDefense interfaces to listen to messages.
/// Add this to an empty GameObject and give it a name to create a new component.
/// Tweak the lists in the Inspector tab in the Unity Editor to customize vulnerabilities,
/// and available defenses that can be bought on to it.
/// </summary>
public class GameNetworkComponent : MonoBehaviour, IUnderAttack, IAddDefense, IDiscover, IAnalyze, IProbe
{
    [Header("Lists")]
    public List<AttackTypes> vulnerabilities;
    public List<DefenseTypes> availableDefenses;
    public List<DefenseTypes> implementedDefenses;

    [Header("Variables")]
    [Range(1f, 5f)]
    public int difficulty = 1;
    private bool visible = false;

    [SerializeField]
    private GameObject uiElement;
    [SerializeField]
    private Button uiButton;
    [SerializeField]
    private AttackerUI uiScript;

    [Header("Network depth")]
    public int graphDepth;
    public List<GameNetworkComponent> children;

    public void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 pos = new Vector3(rectTransform.localPosition.x + rectTransform.rect.height * 1.5f, rectTransform.localPosition.y);
        uiButton.onClick.AddListener(() => uiScript.ToggleOnClickMenu(true, pos));
    }

    public void Update()
    {
        uiElement.SetActive(visible);
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
                            if (VulnerabilityPairings.IsStoppedBy(message.attack, def))
                                isVulnerable = false;
                        }
                        if (Random.Range(0f, 1f) < message.probability * (1f / difficulty))
                            isVulnerable = false;
                    }
                }
                MessagingManager.BroadcastMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.ATTACK_RESPONSE, isVulnerable));
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
                if (availableDefenses.Remove(message.defense))
                {
                    implementedDefenses.Add(message.defense);
                    MessagingManager.BroadcastMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.DEFENSE_RESPONSE, true));
                }
                else
                    MessagingManager.BroadcastMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.DEFENSE_RESPONSE, false));
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
        if (message.depth == graphDepth)
        {
            List<GameNetworkComponent> list = new List<GameNetworkComponent>();

            if (message.targetName == "")
            {
                if (!visible)
                {
                    visible = true;
                    list.Add(this);
                }
                MessagingManager.BroadcastMessage(new DiscoverResponseMessage(message.senderName, name, MessageTypes.Game.DISCOVER_RESPONSE, list));
            }
            else if (message.targetName == name)
            {
                foreach (var child in children)
                {
                    if (child.visible == false && Random.Range(0f, 1f) < (message.probability * (1f / difficulty)))
                    {
                        list.Add(child);
                        child.visible = true;
                    }
                }
                MessagingManager.BroadcastMessage(new DiscoverResponseMessage(message.senderName, name, MessageTypes.Game.DISCOVER_RESPONSE, list));
            }
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
            foreach (var vuln in vulnerabilities)
            {
                bool vulnerable = false;

                if (Random.Range(0f, 1f) < message.probability * (1f / difficulty))
                    vulnerable = true;

                foreach (var def in implementedDefenses)
                {
                    if (VulnerabilityPairings.IsStoppedBy(vuln, def))
                        vulnerable = false;
                }
                if (vulnerable)
                    vulnList.Add(vuln);
            }
            MessagingManager.BroadcastMessage(new AnalyzeResponeMessage(message.senderName, name, MessageTypes.Game.ANALYZE_RESPONE, vulnList));
        }

    }

    public void OnProbe(Message message)
    {
        if (message.targetName == name)
        {
            MessagingManager.BroadcastMessage(new ProbeResponseMessage(message.senderName, name, MessageTypes.Game.PROBE_RESPONSE, children.Count, difficulty, vulnerabilities.Count));
        }
    }
}
