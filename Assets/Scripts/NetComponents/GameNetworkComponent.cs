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
public class GameNetworkComponent : MonoBehaviour, IUnderAttack, IAddDefense, IDiscover
{
    public List<AttackTypes> vulnerabilities;
    public List<DefenseTypes> availableDefenses;
    public List<DefenseTypes> implementedDefenses;

    public bool visible = false;

    public GameObject uiElement;

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
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void UnderAttack(Message message)
    {
        if (name == message.targetName)
        {
            Debug.Log("Message recieved from: " + message.senderName + " to me: " + name);

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
                    }
                }
                MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Game.ATTACK_RESPONSE, isVulnerable));
            }
        }
    }

    /// <summary>
    /// From the IAddDefense interface
    /// 
    /// Listens to a MessageTypes.Events.DEFEND.
    /// Checks if self is the target of this event and if the DefendType is valid.
    /// Removes the defense from availableDefenses list if possible, and add it to the
    /// implementedDefenses list.
    /// Broadcasts message regarding the success of adding the defense.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void AddDefense(Message message)
    {
        if (name == message.targetName)
        {
            Debug.Log("Defense received from: " + message.senderName + " to me: " + name);

            if (message.defense != DefenseTypes.zero)
            {
                if (availableDefenses.Remove(message.defense))
                {
                    implementedDefenses.Add(message.defense);
                    MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Game.DEFENSE_RESPONSE, true));
                }
                else
                    MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Game.DEFENSE_RESPONSE, false));
            }
        }
    }

    public void OnDiscover(Message message)
    {
        Debug.Log("Discover received from: " + message.senderName + " to me: " + name);
        MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Game.DISCOVER_RESPONSE, true));
        visible = true;
    }
}
