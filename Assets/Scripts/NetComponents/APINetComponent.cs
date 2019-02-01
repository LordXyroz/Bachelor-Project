using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class APINetComponent : BaseNetComponent, IUnderAttack, IAddDefense
{
    public override void Start()
    {
        availableDefenses = new List<DefenseEnum>();
        implementedDefenses = new List<DefenseEnum>();

        availableDefenses.Add(DefenseEnum.Sanitize_Input);
    }

    public override void Update()
    {
        //throw new System.NotImplementedException();
    }

    public void UnderAttack(Message message)
    {
        if (name == message.targetName)
        {
            Debug.Log("Message recieved from: " + message.senderName + " to me: " + name);

            if (message.attack != AttackEnum.zero)
            {
                bool isVulnerable = false;
                foreach (var vuln in vulnerabilities)
                {
                    if (message.attack == vuln)
                    {
                        isVulnerable = true;
                        foreach (var def in implementedDefenses)
                        {
                            if (def == DefenseEnum.Sanitize_Input)
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

            if (message.defense != DefenseEnum.zero)
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


