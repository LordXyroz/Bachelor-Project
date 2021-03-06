﻿/// <summary>
/// Namespace for containing every type of message to be sent within the game.
/// Types are categorized in static classes where each category has its own offset.
/// All message types are set in ushort to reduce size (2 bytes vs 4 with uint), 
/// and we don't need millions of event types. With ushort, we can have a maximum of 
/// 65'535 different events. The offsets are set to the 2nd byte to have 256 types per category
/// </summary>
namespace MessageTypes
{
    using System.Collections.Generic;

    /// <summary>
    /// Static class for logging event codes
    /// </summary>
    [System.Serializable]
    public static class Logging
    {
        private const ushort offset = 0x0000;

        public const ushort Log = offset + 1;
        public const ushort Debug = offset + 2;
        public const ushort Warning = offset + 3;
        public const ushort Error = offset + 4;
        public const ushort Targeting = offset + 5;

    }

    /// <summary>
    /// Static class for networking event codes
    /// </summary>
    [System.Serializable]
    public static class Network
    {
        private const ushort offset = 0x0100;

        public const ushort Connect = offset + 1;
        public const ushort ConnectAck = offset + 2;

        public const ushort Disconnect = offset + 3;
        public const ushort ClientDisconnect = offset + 4;

        public const ushort Ping = offset + 5;
        public const ushort PingAck = offset + 6;

        public const ushort Chat = offset + 7;

        public const ushort Swap = offset + 8;

    }

    /// <summary>
    /// Static class for game event codes
    /// </summary>
    [System.Serializable]
    public static class Game
    {
        private const ushort offset = 0x0200;

        public const ushort Start = offset + 1;

        public const ushort Attack = offset + 2;
        public const ushort AttackResponse = offset + 3;
        public const ushort Defense = offset + 4;
        public const ushort DefenseResponse = offset + 5;
        public const ushort Discover = offset + 6;
        public const ushort DiscoverResponse = offset + 7;
        public const ushort Analyze = offset + 8;
        public const ushort AnalyzeResponse = offset + 9;
        public const ushort Probe = offset + 10;
        public const ushort ProbeResponse = offset + 11;

        public const ushort SaveFile = offset + 12;
    }

    /// <summary>
    /// Static class for containing the names of each messagetype.
    /// </summary>
    public static class TypeValues {
        private static Dictionary<ushort, string> _KeyValues;

        /// <summary>
        /// Constructor for initializing dictionary
        /// </summary>
        static TypeValues()
        {
            _KeyValues = new Dictionary<ushort, string>
            {
                { Logging.Log, "Log" },
                { Logging.Debug, "Debug" },
                { Logging.Warning, "Warning" },
                { Logging.Error, "Error" },
                { Logging.Targeting, "Targeting" },

                { Network.Connect, "Connect" },
                { Network.ConnectAck, "Connect Acknowledge" },
                { Network.Disconnect, "Disconnect" },
                { Network.ClientDisconnect, "Client Disconnect" },
                { Network.Ping, "Ping" },
                { Network.PingAck, "Pick Acknowledge" },
                { Network.Chat, "Chat" },
                { Network.Swap, "Swap" },

                { Game.Attack, "Attack" },
                { Game.AttackResponse, "Attack Response" },
                { Game.Defense, "Defense" },
                { Game.DefenseResponse, "Defense Response" },
                { Game.Discover, "Discover" },
                { Game.DiscoverResponse, "Discover Response" },
                { Game.Analyze, "Analyze" },
                { Game.AnalyzeResponse, "Analyze Response" },
                { Game.Probe, "Probe" },
                { Game.ProbeResponse, "Probe Response" },
                { Game.SaveFile, "Save file" }
            };
        }

        /// <summary>
        /// Gets the string paired with the messagetype
        /// </summary>
        /// <param name="type">Messagetype</param>
        /// <returns>String paired with messagetype, or "Not implemented" if none are found</returns>
        public static string GetValue(ushort type)
        {
            if (_KeyValues.ContainsKey(type))
                return _KeyValues[type];
            else
                return "Not implemented";
        }
    }
}