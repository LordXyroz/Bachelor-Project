namespace EventTypes
{
    [System.Serializable]
    public static class Attacks
    {
        public const uint INJECTION = 0x00000001;
        public const uint CREDENTIAL_STUFFING = 0x00000002;
        public const uint PACKAGE_SNIFFING = 0x00000003;
        public const uint HOSTILE_XML = 0x00000004;
        public const uint BYPASS_ACCESS_CONTROLL = 0x00000005;
        public const uint EXPLOIT_SEC_MISCONFIG = 0x00000006;
        public const uint XSS = 0x00000007;
        
    }

    [System.Serializable]
    public static class Defenses
    {
        public const uint SANITIZE_INPUT = 0x00000101;
        public const uint FIREWALL = 0x00000102;
    }

    [System.Serializable]
    public static class User_Defined
    {
        public const uint USER_01 = 0x00FF0001;
        public const uint USER_02 = 0x00FF0002;
        public const uint USER_03 = 0x00FF0003;
        public const uint USER_04 = 0x00FF0004;
        public const uint USER_05 = 0x00FF0005;
        public const uint USER_06 = 0x00FF0006;
        public const uint USER_07 = 0x00FF0007;
        public const uint USER_08 = 0x00FF0008;
        public const uint USER_09 = 0x00FF0009;
        public const uint USER_10 = 0x00FF000A;
    }
}