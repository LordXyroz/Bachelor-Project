using UnityEngine;
using System.Linq;

public static class MessagingManager
{
    // Sends a message to all of the different gameobject types
    // Gameobject/Script type is based on event type
    public static void BroadcastMessage(Message message)
    {
        switch (message.messageType)
        {
            case MessageTypes.Events.ATTACK:
                {
                    Debug.Log("Attack event of type: " + message.attack);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("UnderAttack", message);
                    break;
                }

            case MessageTypes.Events.ATTACK_RESPONSE:
                {
                    Debug.Log("Attack response of type: " + message.success);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("AttackResponse", message);
                    break;
                }

            case MessageTypes.Events.DEFENSE:
                {
                    Debug.Log("Defend event of type: " + message.defense);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("AddDefense", message);
                    break;
                }

            case MessageTypes.Events.DEFENSE_RESPONSE:
                {
                    Debug.Log("Defend response of type: " + message.success);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDefenseResponse>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("DefenseResponse", message);
                    break;
                }

            default:
                throw new System.Exception("Unimplemented target!");
        }
    }
}