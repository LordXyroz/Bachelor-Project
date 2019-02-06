﻿/// <summary>
/// Namespace for containing every type of message to be sent within the game.
/// Types are categorized in static classes where each category has its own offset.
/// All message types are set in ushort to reduce size (2 bytes vs 4 with uint), 
/// and we don't need millions of event types. With ushort, we can have a maximum of 
/// 65'535 different events. The offsets are set to the 2nd byte to have 256 types per category
/// </summary>
namespace MessageTypes
{
    /// <summary>
    /// Static class for logging event codes
    /// </summary>
    [System.Serializable]
    public static class Logging
    {
        private const ushort offset = 0x0000;


    }

    /// <summary>
    /// Static class for networking event codes
    /// </summary>
    [System.Serializable]
    public static class Network
    {
        private const ushort offset = 0x0100;


    }

    /// <summary>
    /// Static class for game event codes
    /// </summary>
    [System.Serializable]
    public static class Game
    {
        private const ushort offset = 0x0200;

        public const ushort ATTACK = offset + 1;
        public const ushort ATTACK_RESPONSE = offset + 2;
        public const ushort DEFENSE = offset + 3;
        public const ushort DEFENSE_RESPONSE = offset + 4;
        public const ushort DISCOVER = offset + 5;
        public const ushort DISCOVER_RESPONSE = offset + 6;
        public const ushort ANALYZE = offset + 7;
        public const ushort ANALYZE_RESPONE = offset + 8;
    }

    [System.Serializable]
    public static class Attacks
    {
        private const ushort offset = 0x0400;

        public const ushort INJECTION = offset + 1;
        public const ushort CREDENTIAL_STUFFING = offset + 2;
        public const ushort PACKAGE_SNIFFING = offset + 3;
        public const ushort HOSTILE_XML = offset + 4;
        public const ushort BYPASS_ACCESS_CONTROLL = offset + 5;
        public const ushort EXPLOIT_SEC_MISCONFIG = offset + 6;
        public const ushort XSS = offset + 7;
        public const ushort REMOTE_CODE_EXEC = offset + 8;
        public const ushort EXPLOIT_MISCONFIG_SYS = offset + 9;
        public const ushort DISABLE_LOG = offset + 10;
    }

    [System.Serializable]
    public static class Defenses
    {
        private const ushort offset = 0x0500;

        public const ushort SANITIZE_INPUT = offset + 1;
        public const ushort TWO_FACTOR_AUTH = offset + 2;
        public const ushort FIREWALL = offset + 3;
        public const ushort PATCH_UPDATE = offset + 4;
        public const ushort LOG_FAILURE = offset + 5;
        public const ushort UPDATE_CONFIG = offset + 6;
        public const ushort ESCAPE_UNTRUSTED_HTTP_DATA = offset + 7;
        public const ushort INTEGRITY_CHECK = offset + 8;
        public const ushort PATCH_SYSTEM = offset + 9;
        public const ushort OFFSITE_BACKUP = offset + 10;
    }

    [System.Serializable]
    public static class User_Defined
    {
        private const ushort offset = 0x0600;

        public const ushort USER_01 = offset + 1;
        public const ushort USER_02 = offset + 2;
        public const ushort USER_03 = offset + 3;
        public const ushort USER_04 = offset + 4;
        public const ushort USER_05 = offset + 5;
        public const ushort USER_06 = offset + 6;
        public const ushort USER_07 = offset + 7;
        public const ushort USER_08 = offset + 8;
        public const ushort USER_09 = offset + 9;
        public const ushort USER_10 = offset + 10;
    }
}