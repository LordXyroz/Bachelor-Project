/// <summary>
/// Namespace for messaging interfaces.
/// Classes that inherits from an interface will automatically get sent messages for that interface during runtime.
/// </summary>
namespace MessagingInterfaces
{
    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.Defense"/>.
    /// </summary>
    public interface IAddDefense
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.Defense"/> event happens.
        /// </summary>
        /// <param name="message">The defense message</param>
        void AddDefense(DefenseMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.Analyze"/>.
    /// </summary>
    public interface IAnalyze
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.Analyze"/> event happens.
        /// </summary>
        /// <param name="message">Analyze message</param>
        void OnAnalyze(AnalyzeMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.AnalyzeResponse"/>.
    /// </summary>
    public interface IAnalyzeResponse
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.AnalyzeResponse"/> event happens.
        /// </summary>
        /// <param name="message">Analyze response message</param>
        void OnAnalyzeResponse(AnalyzeResponeMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.AttackResponse"/>.
    /// </summary>
    public interface IAttackResponse
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.AttackResponse"/> event happens.
        /// </summary>
        /// <param name="message">Success message</param>
        void AttackResponse(SuccessMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Network.Connect"/>.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Network.Connect"/> event happens.
        /// </summary>
        /// <param name="message">Connect message</param>
        /// <param name="index">Conection index of client on the server</param>
        void OnConnection(ConnectMessage message, int index);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Network.Disconnect"/>.
    /// </summary>
    public interface IDisconect
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Network.Disconnect"/> event happens.
        /// </summary>
        void OnDisconnect();
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Network.ClientDisconnect"/>.
    /// </summary>
    public interface IDisconnectClient
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Network.ClientDisconnect"/> event happens.
        /// </summary>
        /// <param name="message">Disconnect client message</param>
        void OnClientDisconnect(DiscClientMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Network.Chat"/>.
    /// </summary>
    public interface IChatMessage
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Network.Chat"/> event happens.
        /// </summary>
        /// <param name="message">Chat message</param>
        void OnChatMessage(ChatMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.DefenseResponse"/>.
    /// </summary>
    public interface IDefenseResponse
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.DefenseResponse"/> event happens.
        /// </summary>
        /// <param name="message">Success message</param>
        void DefenseResponse(SuccessMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.Discover"/>.
    /// </summary>
    public interface IDiscover
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.Discover"/> event happens.
        /// </summary>
        /// <param name="message">Discover message</param>
        void OnDiscover(DiscoverMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.DiscoverResponse"/>.
    /// </summary>
    public interface IDiscoverResponse
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.DiscoverResponse"/> event happens.
        /// </summary>
        /// <param name="message">Discover response message</param>
        void OnDiscoverResponse(DiscoverResponseMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Logging.Error"/>.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Logging.Error"/> event happens.
        /// </summary>
        /// <param name="message">Logging message</param>
        void OnError(LoggingMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Logging.Log"/>.
    /// </summary>
    public interface ILogging
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Logging.Log"/> event happens.
        /// </summary>
        /// <param name="message">Logging message</param>
        void OnLog(LoggingMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Network.Ping"/>.
    /// </summary>
    public interface IPing
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Network.Ping"/> event happens.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="index">Connection index on server</param>
        void OnPing(Message message, int index);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.Probe"/>.
    /// </summary>
    public interface IProbe
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.Probe"/> event happens.
        /// </summary>
        /// <param name="message">Message</param>
        void OnProbe(Message message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.ProbeResponse"/>.
    /// </summary>
    public interface IProbeResponse
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.ProbeResponse"/> event happens.
        /// </summary>
        /// <param name="message">Probe response message.</param>
        void OnProbeResponse(ProbeResponseMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.SaveFile"/>.
    /// </summary>
    public interface ISaveFile
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.SaveFile"/> event happens.
        /// </summary>
        /// <param name="message">Save file message</param>
        void OnSaveFile(SaveFileMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.Start"/>.
    /// </summary>
    public interface IStartGame
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.Start"/> event happens.
        /// </summary>
        void OnStartGame();
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Network.Swap"/>.
    /// </summary>
    public interface ISwap
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Network.Swap"/> event happens.
        /// </summary>
        /// <param name="message"></param>
        void OnSwap(SwapMessage message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Logging.Targeting"/>.
    /// </summary>
    public interface ITargeting
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Logging.Targeting"/> event happens.
        /// </summary>
        /// <param name="message">Message</param>
        void OnTargeting(Message message);
    }

    /// <summary>
    /// Interface that listens to <see cref="MessageTypes.Game.Attack"/>.
    /// </summary>
    public interface IUnderAttack
    {
        /// <summary>
        /// Function that gets called when a <see cref="MessageTypes.Game.Attack"/> event happens.
        /// </summary>
        /// <param name="message">Attack message</param>
        void UnderAttack(AttackMessage message);
    }
}
