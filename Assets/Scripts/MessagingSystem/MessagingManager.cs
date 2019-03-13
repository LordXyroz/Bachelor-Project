using UnityEngine;
using System.Linq;
using MessagingInterfaces;

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
    public static void BroadcastMessage(Message message, int index = -1)
    {
        switch (message.messageType)
        {
            case MessageTypes.Game.Attack:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (var o in objects)
                        o.UnderAttack((AttackMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((AttackMessage) message);
                    break;
                }

            case MessageTypes.Game.AttackResponse:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (var o in objects)
                        o.AttackResponse((SuccessMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((SuccessMessage) message);
                    break;
                }

            case MessageTypes.Game.Defense:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (var o in objects)
                        o.AddDefense((DefenseMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((DefenseMessage) message);
                    break;
                }

            case MessageTypes.Game.DefenseResponse:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDefenseResponse>();
                    foreach (var o in objects)
                        o.DefenseResponse((SuccessMessage) message);


                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((SuccessMessage) message);
                    break;
                }

            case MessageTypes.Game.Discover:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDiscover>();
                    foreach (var o in objects)
                        o.OnDiscover((DiscoverMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((DiscoverMessage) message);
                    break;
                }

            case MessageTypes.Game.DiscoverResponse:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDiscoverResponse>();
                    foreach (var o in objects)
                        o.OnDiscoverResponse((DiscoverResponseMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((DiscoverResponseMessage) message);
                    break;
                }

            case MessageTypes.Game.Analyze:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAnalyze>();
                    foreach (var o in objects)
                        o.OnAnalyze((AnalyzeMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((AnalyzeMessage) message);
                    break;
                }

            case MessageTypes.Game.AnalyzeResponse:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IAnalyzeResponse>();
                    foreach (var o in objects)
                        o.OnAnalyzeResponse((AnalyzeResponeMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((AnalyzeResponeMessage) message);
                    break;
                }

            case MessageTypes.Game.Probe:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IProbe>();
                    foreach (var o in objects)
                        o.OnProbe(message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save(message);
                    break;
                }

            case MessageTypes.Game.ProbeResponse:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IProbeResponse>();
                    foreach (var o in objects)
                        o.OnProbeResponse((ProbeResponseMessage) message);

                    if (Object.FindObjectOfType<PlayerManager>().GetPlayerType() == PlayerManager.PlayerType.Observer)
                        LogSaving.Save((ProbeResponseMessage) message);
                    break;
                }

            case MessageTypes.Logging.Log:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ILogging>();
                    foreach (var o in objects)
                        o.OnLog((LoggingMessage) message);
                    break;
                }

            case MessageTypes.Logging.Error:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IError>();
                    foreach (var o in objects)
                        o.OnError((LoggingMessage) message);
                    break;
                }

            case MessageTypes.Logging.Targeting:
                {
                    var objects = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ITargeting>();
                    foreach (var o in objects)
                        o.OnTargeting(message);
                    break;
                }

            case MessageTypes.Network.Ping:
                {
                    var netManager = GameObject.FindObjectOfType<NetworkingManager>();

                    if (netManager.cb != null)
                        netManager.cb.OnConnection(message, index);
                    else if (netManager.sb != null)
                        netManager.sb.OnConnection(message, index);

                    break;
                }

            default:
                string msg = "Unimplemented event sent!";
                BroadcastMessage(new LoggingMessage("", message.senderName, MessageTypes.Logging.Error, msg));
                break;
        }
    }
}