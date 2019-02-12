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
    /// Casts the message to the required type for each interface.
    /// Calls the interface function for each receiver.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public static void BroadcastMessage(Message message)
    {
        switch (message.messageType)
        {
            case MessageTypes.Game.ATTACK:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (var o in objects)
                        o.UnderAttack((AttackMessage) message);
                    break;
                }

            case MessageTypes.Game.ATTACK_RESPONSE:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (var o in objects)
                        o.AttackResponse((SuccessMessage) message);
                    break;
                }

            case MessageTypes.Game.DEFENSE:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (var o in objects)
                        o.AddDefense((DefenseMessage) message);
                    break;
                }

            case MessageTypes.Game.DEFENSE_RESPONSE:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDefenseResponse>();
                    foreach (var o in objects)
                        o.DefenseResponse((SuccessMessage) message);
                    break;
                }

            case MessageTypes.Game.DISCOVER:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDiscover>();
                    foreach (var o in objects)
                        o.OnDiscover((DiscoverMessage) message);
                    break;
                }

            case MessageTypes.Game.DISCOVER_RESPONSE:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDiscoverResponse>();
                    foreach (var o in objects)
                        o.OnDiscoverResponse((DiscoverResponseMessage) message);
                    break;
                }

            case MessageTypes.Game.ANALYZE:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAnalyze>();
                    foreach (var o in objects)
                        o.OnAnalyze(message);
                    break;
                }

            case MessageTypes.Game.ANALYZE_RESPONE:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAnalyzeResponse>();
                    foreach (var o in objects)
                        o.OnAnalyzeResponse((AnalyzeResponeMessage) message);
                    break;
                }

            case MessageTypes.Logging.LOG:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ILogging>();
                    foreach (var o in objects)
                        o.OnLog((LoggingMessage) message);
                    break;
                }

            default:
                throw new System.Exception("Unimplemented target!");
        }
    }
}