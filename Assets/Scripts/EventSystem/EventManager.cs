using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    // Sends a message all of the different gameobject types
    // Gameobject/Script type is based on event type
    public static void BroadcastMessage(Message message)
    {
        switch (message.messageType)
        {
            case MessageTypes.Events.ATTACK:
                {
                    Debug.Log("Attack event sent!");
                    var objects = GameObject.FindObjectsOfType<BaseNetComponent>();
                    foreach (var o in objects)
                        o.SendMessage("UnderAttack", message);
                    break;
                }

            case MessageTypes.Events.DEFENSE:
                {
                    Debug.Log("Defend event sent!");
                    var objects = GameObject.FindObjectsOfType<BaseAttack>();
                    foreach (var o in objects)
                        o.SendMessage("AttackResponse", message);
                    break;
                }

            default:
                throw new System.Exception("Unimplemented target!");
        }
    }
}