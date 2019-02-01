using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkComponent : MonoBehaviour, IUnderAttack, IAddDefense
{
    public List<AttackTypes> vulnerabilities;
    public List<DefenseTypes> availableDefenses;
    public List<DefenseTypes> implementedDefenses;

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
                            if (VulnerabilityPairings.IsVulnerable(message.attack, def))
                                isVulnerable = false;
                        }
                    }
                }
                MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Events.ATTACK_RESPONSE, isVulnerable));
            }
        }
    }

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
                    MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Events.DEFENSE_RESPONSE, true));
                }
                else
                    MessagingManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Events.DEFENSE_RESPONSE, false));
            }
        }
    }
}
