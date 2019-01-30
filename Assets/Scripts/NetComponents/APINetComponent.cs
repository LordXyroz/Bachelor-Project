using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APINetComponent : BaseNetComponent
{
    public override void Start()
    {
        vulnerability = new List<BaseAttack>();
        availableDefenses = new List<BaseDefense>();
        implementedDefenses = new List<BaseDefense>();
    }

    public override void Update()
    {
        //throw new System.NotImplementedException();
    }

    public override void UnderAttack(Message message)
    {
        if (name == message.targetName)
        {
            Debug.Log("Message recieved from: " + message.senderName + " to me: " + name);

            EventManager.BroadcastMessage(new Message(message.senderName, name, MessageTypes.Events.DEFENSE));
        }
    }
}
