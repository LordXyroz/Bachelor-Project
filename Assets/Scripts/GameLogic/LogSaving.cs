using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LogSaving
{
    static StreamWriter attackerWriter;
    static StreamWriter defenderWriter;

    static LogSaving()
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(Destructor);

        string path = Application.persistentDataPath + "/Logs/";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        attackerWriter = new StreamWriter(path + "Attacker" + System.DateTime.Now + ".txt");
        attackerWriter.WriteLine("Event:");

        defenderWriter = new StreamWriter(path + "Defender" + System.DateTime.Now + ".txt");
        defenderWriter.WriteLine("Event:");
    }

    public static void Destructor(object sender, EventArgs e)
    {
        attackerWriter.Close();
        defenderWriter.Close();
    }
    
    public static void Save(AnalyzeMessage message)
    {
        Save((Message) message);

        if (message.targetName == "Attacker" || message.senderName == "Attacker")
        {
            attackerWriter.WriteLine("\tProbability: " + message.probability);
        }
        else if (message.targetName == "Defender" || message.senderName == "Defender")
        {
            defenderWriter.WriteLine("\tProbability: " + message.probability);
        }
    }

    public static void Save(AnalyzeResponeMessage message)
    {
        Save((Message) message);

        string log = "\tVulnerabilities: ";
        foreach (var v in message.attacks)
            log += v + "  ";

        if (message.targetName == "Attacker" || message.senderName == "Attacker")
        {
            attackerWriter.WriteLine(log);
        }
        else if (message.targetName == "Defender" || message.senderName == "Defender")
        {
            defenderWriter.WriteLine(log);
        }
    }

    public static void Save(AttackMessage message)
    {
        Save((Message) message);

        if (message.targetName == "Attacker" ||
            message.senderName == "Attacker" ||
            message.messageType == MessageTypes.Game.Attack ||
            message.messageType == MessageTypes.Game.AttackResponse)
        {
            attackerWriter.WriteLine("\tAttack: " + message.attack);
        }
    }

    public static void Save(DefenseMessage message)
    {
        Save((Message) message);
        
        if (message.targetName == "Defender" || 
            message.senderName == "Defender" ||
            message.messageType == MessageTypes.Game.Defense ||
            message.messageType == MessageTypes.Game.DefenseResponse)
        {
            defenderWriter.WriteLine("\tDefense: " + message.defense);
        }
    }

    public static void Save(DiscoverMessage message)
    {
        Save((Message) message);

        if (message.targetName == "Attacker" || message.senderName == "Attacker")
        {
            attackerWriter.WriteLine("\tProbability: " + message.probability);
        }
    }

    public static void Save(DiscoverResponseMessage message)
    {
        Save((Message) message);

        if (message.targetName == "Attacker" || message.senderName == "Attacker")
        {
            string log = "\tDiscovered: ";
            
            if (message.discovered != null)
            {
                foreach (var o in message.discovered)
                    log += o + "  ";
            }
            else
                log += message.senderName;

            attackerWriter.WriteLine(log);
        }
    }
    
    public static void Save(Message message)
    {
        if (message.targetName == "Attacker" ||
            message.senderName == "Attacker" ||
            message.messageType == MessageTypes.Game.Attack ||
            message.messageType == MessageTypes.Game.AttackResponse)
        {
            attackerWriter.WriteLine("\n  - Event_type: " + MessageTypes.TypeValues.GetValue(message.messageType));
            attackerWriter.WriteLine("\tFrom: " + message.senderName);
            if (message.targetName != "")
                attackerWriter.WriteLine("\tTo: " + message.targetName);
            else
                attackerWriter.WriteLine("\tTo: Root");
        }
        else if (message.targetName == "Defender" ||
            message.senderName == "Defender" ||
            message.messageType == MessageTypes.Game.Defense ||
            message.messageType == MessageTypes.Game.DefenseResponse)
        {
            defenderWriter.WriteLine("\n  - Event_type: " + MessageTypes.TypeValues.GetValue(message.messageType));
            defenderWriter.WriteLine("\tFrom: " + message.senderName);
            if (message.targetName != "")
                defenderWriter.WriteLine("\tTo: " + message.targetName);
            else
                defenderWriter.WriteLine("\tTo: Root");
        }
    }

    public static void Save(ProbeResponseMessage message)
    {
        Save((Message) message);

        if (message.targetName == "Attacker" || message.senderName == "Attacker")
        {
            attackerWriter.WriteLine("\tDifficulty: " + message.difficulty);
            attackerWriter.WriteLine("\tChild nodes: " + message.numOfChildren);
            attackerWriter.WriteLine("\tNumber of Vulnerabilities: " + message.numOfVulnerabilities);
        }
        else if (message.targetName == "Defender" || message.senderName == "Defender")
        {
            defenderWriter.WriteLine("\tDifficulty: " + message.difficulty);
            defenderWriter.WriteLine("\tChild nodes: " + message.numOfChildren);
            defenderWriter.WriteLine("\tNumber of Vulnerabilities: " + message.numOfVulnerabilities);
        }
    }

    public static void Save(SuccessMessage message)
    {
        Save((Message) message);

        if (message.messageType == MessageTypes.Game.AttackResponse)
        {
            attackerWriter.WriteLine("\tSuccess: " + message.success);
        }
        else if (message.messageType == MessageTypes.Game.DefenseResponse)
        {
            defenderWriter.WriteLine("\tSuccess: " + message.success);
        }
    }
}
