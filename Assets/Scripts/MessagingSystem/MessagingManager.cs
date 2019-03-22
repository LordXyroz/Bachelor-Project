using UnityEngine;
using System.Linq;
using MessagingInterfaces;

/// <summary>
/// Static class to handle messaging between GameObjects in the scene.
/// </summary>
public static class MessagingManager
{
    private static NetworkingManager _NetworkingManager = null;
    
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
        if (_NetworkingManager == null)
            _NetworkingManager = Object.FindObjectOfType<NetworkingManager>();

        switch (message.messageType)
        {
            case MessageTypes.Game.Attack:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IUnderAttack>();
                    foreach (var o in objects)
                        o.UnderAttack((AttackMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((AttackMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.AttackResponse:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IAttackResponse>();
                    foreach (var o in objects)
                        o.AttackResponse((SuccessMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((SuccessMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.Defense:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IAddDefense>();
                    foreach (var o in objects)
                        o.AddDefense((DefenseMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((DefenseMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.DefenseResponse:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDefenseResponse>();
                    foreach (var o in objects)
                        o.DefenseResponse((SuccessMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((SuccessMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.Discover:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDiscover>();
                    foreach (var o in objects)
                        o.OnDiscover((DiscoverMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((DiscoverMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.DiscoverResponse:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDiscoverResponse>();
                    foreach (var o in objects)
                        o.OnDiscoverResponse((DiscoverResponseMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((DiscoverResponseMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.Analyze:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IAnalyze>();
                    foreach (var o in objects)
                        o.OnAnalyze((AnalyzeMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((AnalyzeMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.AnalyzeResponse:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IAnalyzeResponse>();
                    foreach (var o in objects)
                        o.OnAnalyzeResponse((AnalyzeResponeMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((AnalyzeResponeMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.Start:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnStartGame();
                    break;
                }

            case MessageTypes.Game.Probe:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IProbe>();
                    foreach (var o in objects)
                        o.OnProbe(message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save(message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.ProbeResponse:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IProbeResponse>();
                    foreach (var o in objects)
                        o.OnProbeResponse((ProbeResponseMessage) message);

                    if (_NetworkingManager.playerType == PlayerManager.PlayerType.Observer)
                    {
                        LogSaving.Save((ProbeResponseMessage)message);
                        _NetworkingManager.sb.BroadcastMessage(message);
                    }
                    break;
                }

            case MessageTypes.Game.SaveFile:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnSaveFile((SaveFileMessage) message);
                    break;
                }

            case MessageTypes.Logging.Log:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<ILogging>();
                    foreach (var o in objects)
                        o.OnLog((LoggingMessage) message);
                    break;
                }

            case MessageTypes.Logging.Error:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IError>();
                    foreach (var o in objects)
                        o.OnError((LoggingMessage) message);
                    break;
                }

            case MessageTypes.Logging.Targeting:
                {
                    var objects = Object.FindObjectsOfType<MonoBehaviour>().OfType<ITargeting>();
                    foreach (var o in objects)
                        o.OnTargeting(message);
                    break;
                }

            case MessageTypes.Network.Connect:
                {
                    if (_NetworkingManager.sb != null)
                        _NetworkingManager.sb.OnConnection((ConnectMessage) message, index);

                    break;
                }

            case MessageTypes.Network.ConnectAck:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnConnection((ConnectMessage)message, index);

                    break;
                }

            case MessageTypes.Network.ClientDisconnect:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnClientDisconnect((DiscClientMessage) message);

                    break;
                }

            case MessageTypes.Network.Disconnect:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnDisconnect();
                    break;
                }

            case MessageTypes.Network.Ping:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnPing(message, index);
                    else if (_NetworkingManager.sb != null)
                        _NetworkingManager.sb.OnPing(message, index);

                    break;
                }

            case MessageTypes.Network.PingAck:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnPing(message, index);
                    else if (_NetworkingManager.sb != null)
                        _NetworkingManager.sb.OnPing(message, index);

                    break;
                }

            case MessageTypes.Network.Chat:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnChatMessage((ChatMessage) message);
                    else if (_NetworkingManager.sb != null)
                        _NetworkingManager.sb.OnChatMessage((ChatMessage) message);

                    break;
                }

            case MessageTypes.Network.Swap:
                {
                    if (_NetworkingManager.cb != null)
                        _NetworkingManager.cb.OnSwap((SwapMessage) message);
                    else if (_NetworkingManager.sb != null)
                        _NetworkingManager.sb.OnSwap((SwapMessage) message);
                    
                    break;
                }

            default:
                string msg = "Unimplemented event sent!";
                BroadcastMessage(new LoggingMessage("", message.senderName, MessageTypes.Logging.Error, msg));
                break;
        }
    }
}