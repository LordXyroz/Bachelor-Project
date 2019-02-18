public class ProbeResponseMessage : Message
{
    public int numOfChildren;
    public int difficulty;

    public ProbeResponseMessage(string target, string sender, ushort type, int num, int diff)
        : base(target, sender, type)
    {
        numOfChildren = num;
        difficulty = diff;
    }
}
