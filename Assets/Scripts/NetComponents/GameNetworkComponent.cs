using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MessagingInterfaces;

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

    [SerializeField]
    private GameObject uiElement;
    [SerializeField]
    private Button uiButton;
    [SerializeField]
    private BaseUI uiScript;

    [Header("Network depth")]
    public int graphDepth;
    public List<GameNetworkComponent> children;

    /// <summary>
    /// Sets up the OnClick of the button attached to this object.
    /// </summary>
    public void Start()
    {
        uiScript = FindObjectOfType<PlayerManager>().GetUIScript();

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 pos = new Vector3(rectTransform.position.x + rectTransform.rect.height * 1.5f, rectTransform.position.y);
        uiButton.onClick.AddListener(uiScript.DisablePopupWindow);
        uiButton.onClick.AddListener(() => FindObjectOfType<Attacker>().SetTarget(gameObject));
        uiButton.onClick.AddListener(() => FindObjectOfType<Defender>().SetTarget(gameObject));
        uiButton.onClick.AddListener(() => uiScript.ToggleOnClickMenu(true, pos));

        if (FindObjectOfType<PlayerManager>().IsAttacker())
            uiElement.SetActive(false);
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
                      
                        if (!(Random.Range(0f, 1f) <= message.probability - (0.8f * (difficulty / 5f - 0.2f))))
                            isVulnerable = false;
                    }
                }
                MessagingManager.BroadcastMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.AttackResponse, isVulnerable));
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
                    MessagingManager.BroadcastMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.DefenseResponse, true));
                }
                else
                    MessagingManager.BroadcastMessage(new SuccessMessage(message.senderName, name, MessageTypes.Game.DefenseResponse, false));
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
                if (!uiElement.activeSelf)
                {
                    uiElement.SetActive(true);
                    list.Add(this);
                }
                MessagingManager.BroadcastMessage(new DiscoverResponseMessage(message.senderName, name, MessageTypes.Game.DiscoverResponse, list));
            }
            else if (message.targetName == name)
            {
                foreach (var child in children)
                {
                    if (!child.uiElement.activeSelf && (Random.Range(0f, 1f) <= message.probability - (0.8f * (difficulty / 5f - 0.2f))))
                    {
                        list.Add(child);
                        child.uiElement.SetActive(true);
                    }
                }
                MessagingManager.BroadcastMessage(new DiscoverResponseMessage(message.senderName, name, MessageTypes.Game.DiscoverResponse, list));
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

                if (Random.Range(0f, 1f) <= message.probability - (0.8f * (difficulty / 5f - 0.2f)))
                    vulnerable = true;

                foreach (var def in implementedDefenses)
                {
                    if (VulnerabilityPairings.IsStoppedBy(vuln, def))
                        vulnerable = false;
                }
                if (vulnerable)
                    vulnList.Add(vuln);
            }
            MessagingManager.BroadcastMessage(new AnalyzeResponeMessage(message.senderName, name, MessageTypes.Game.AnalyzeResponse, vulnList));
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
                    if (VulnerabilityPairings.IsStoppedBy(entry, def))
                        stopped = true;

                if (!stopped)
                    i++;
            }
            
            MessagingManager.BroadcastMessage(new ProbeResponseMessage(message.senderName, name, MessageTypes.Game.ProbeResponse, children.Count, difficulty, i));
        }
    }
    
}
