using UnityEngine;
using System.Linq;

/// <summary>
/// Static class to handle messaging between GameObjects in the scene.
/// </summary>
public static class MessagingManager
{
    /// <summary>
    /// Sends the message to relevant GameObjects based on interfaces and MessageTypes.
    /// Uses FindObjectsOfType to find every MonoBehaviour (scripts that inherit it),
    /// and sorts out only those with the relevant interface using Linq.
    /// Calls the function listening directly.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public static void BroadcastMessage(Message message)
    {
        switch (message.messageType)
        {
            case MessageTypes.Game.ATTACK:
                {
                    Debug.Log("Attack event!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (var o in objects)
                        o.UnderAttack((AttackMessage) message);
                    break;
                }

            case MessageTypes.Game.ATTACK_RESPONSE:
                {
                    Debug.Log("Attack response!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (var o in objects)
                        o.AttackResponse((SuccessMessage) message);
                    break;
                }

            case MessageTypes.Game.DEFENSE:
                {
                    Debug.Log("Defend event!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (var o in objects)
                        o.AddDefense((DefenseMessage) message);
                    break;
                }

            case MessageTypes.Game.DEFENSE_RESPONSE:
                {
                    Debug.Log("Defend response!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDefenseResponse>();
                    foreach (var o in objects)
                        o.DefenseResponse((SuccessMessage) message);
                    break;
                }

            case MessageTypes.Game.DISCOVER:
                {
                    Debug.Log("Discover event!");
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDiscover>();
                    foreach (var o in objects)
                        o.OnDiscover(message);
                    break;
                }

            case MessageTypes.Game.DISCOVER_RESPONSE:
                {
                    Debug.Log("Discover response!");
                    break;
                }

            case MessageTypes.Game.ANALYZE:
                {
                    Debug.Log("Analyze event!");
                    break;
                }

            case MessageTypes.Game.ANALYZE_RESPONE:
                {
                    Debug.Log("Analyze response!");
                    break;
                }

            default:
                throw new System.Exception("Unimplemented target!");
        }
    }
}