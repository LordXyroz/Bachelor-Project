
public class ConnectMessage : Message
{
    public string hostName;
    public string attackerName;
    public string defenderName;

    public ConnectMessage(string target, string sender, ushort type, string host, string attacker, string defender) :
        base(target, sender, type)
    {
        hostName = host;
        attackerName = attacker;
        defenderName = defender;
    }
}
