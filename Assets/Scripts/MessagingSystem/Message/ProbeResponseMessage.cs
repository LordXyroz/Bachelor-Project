public class ProbeResponseMessage : Message
{
    public int numOfVulnerabilities;
    public int numOfChildren;
    public int difficulty;

    public ProbeResponseMessage(string target, string sender, ushort type, int num, int diff, int vuln)
        : base(target, sender, type)
    {
        numOfVulnerabilities = vuln;
        numOfChildren = num;
        difficulty = diff;
    }
}
