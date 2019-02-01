using UnityEngine;
using System.Linq;

/// <summary>
/// Static class to handle messaging between GameObjects.
/// </summary>
public static class MessagingManager
{
    /// <summary>
    /// Sends the message to relevant GameObjects based on interfaces and MessageTypes.
    /// Uses FindObjectsOfType to find every MonoBehaviour (scripts that inherit it),
    /// and sorts out only those with the relevant interface using Linq.
    /// Casts individual items in the IEnumerable to MonoBehaviour in order to 
    /// send the message.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public static void BroadcastMessage(Message message)
    {
        switch (message.messageType)
        {
            case MessageTypes.Game.ATTACK:
                {
                    Debug.Log("Attack event of type: " + message.attack);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("UnderAttack", message);
                    break;
                }

            case MessageTypes.Game.ATTACK_RESPONSE:
                {
                    Debug.Log("Attack response of type: " + message.success);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("AttackResponse", message);
                    break;
                }

            case MessageTypes.Game.DEFENSE:
                {
                    Debug.Log("Defend event of type: " + message.defense);
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (MonoBehaviour o in objects)
                        o.SendMessage("AddDefense", message);
                    break;
                }

            case MessageTypes.Game.DEFENSE_RESPONSE:
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