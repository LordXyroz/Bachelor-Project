using UnityEngine;
using System.Linq;

public static class MessagingManager
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
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("UnderAttack", message);
                    break;
                }

            case MessageTypes.Events.DEFENSE:
                {
                    Debug.Log("Defend event sent!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("AttackResponse", message);
                    break;
                }

            case MessageTypes.Events.ADD_DEFENSE:
                {
                    Debug.Log("Defend event sent!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("AddDefense", message);
                    break;
                }

            default:
                throw new System.Exception("Unimplemented target!");
        }
    }
}