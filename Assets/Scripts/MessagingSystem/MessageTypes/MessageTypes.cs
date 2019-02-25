/// <summary>
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

        public const ushort Log = offset + 1;
        public const ushort Debug = offset + 2;
        public const ushort Warning = offset + 3;
        public const ushort Error = offset + 4;

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

        public const ushort Attack = offset + 1;
        public const ushort AttackResponse = offset + 2;
        public const ushort Defense = offset + 3;
        public const ushort DefenseResponse = offset + 4;
        public const ushort Discover = offset + 5;
        public const ushort DiscoverResponse = offset + 6;
        public const ushort Analyze = offset + 7;
        public const ushort AnalyzeResponse = offset + 8;
        public const ushort Probe = offset + 9;
        public const ushort ProbeResponse = offset + 10;
    }
}