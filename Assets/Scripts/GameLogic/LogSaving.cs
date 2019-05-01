using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Static class used for observer saving events from attacker and defender.
/// </summary>
public static class LogSaving
{
    static bool disposedValue = false;
    static bool initializeValue = false;

    static StreamWriter attackerWriter;
    static StreamWriter defenderWriter;

    /// <summary>
    /// Static constructor.
    /// Creates files in the /Logs/ folder, and creates the folder if it doesn't exist.
    /// </summary>
    static LogSaving()
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(Destructor);
        
        Initialize(false);
    }
    
    /// <summary>
    /// Delegate since C# doesn't have destructors for static classes.
    /// Makes sure the files are closed.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    public static void Destructor(object sender, EventArgs e)
    {
        Dispose(false);
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
    public static void Save(DiscoverMessage message)
    {
        Save((Message) message);

        if (message.targetName == "Attacker" || message.senderName == "Attacker")
        {
            attackerWriter.WriteLine("\tProbability: " + message.probability);
        }

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }
    
    /// <summary>
    /// Base function for saving events from the message class.
    /// Saves events on different files based on attacker or defender event.
    /// </summary>
    /// <param name="message">Base message class containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Overloaded function.
    /// Calls base function before adding extra info.
    /// </summary>
    /// <param name="message">Message containing relevant info</param>
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

        FlushWriters();
    }

    /// <summary>
    /// Flushes writers to make sure things are written to the file.
    /// </summary>
    public static void FlushWriters()
    {
        attackerWriter.Flush();
        defenderWriter.Flush();
    }

    /// <summary>
    /// Public function for calling <see cref="Dispose(bool)"/> manually.
    /// </summary>
    public static void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Public function for calling <see cref="Initialize(bool)"/> manually.
    /// </summary>
    public static void Initialize()
    {
        Initialize(true);
    }

    /// <summary>
    /// Private function for handling closing of writers
    /// </summary>
    /// <param name="disposing"></param>
    static void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }
            FlushWriters();

            attackerWriter.Close();
            defenderWriter.Close();

            disposedValue = true;
            initializeValue = false;
        }
    }

    /// <summary>
    /// Private function for handling creation of new log files
    /// </summary>
    /// <param name="initialized"></param>
    static void Initialize(bool initialized)
    {
        if (!initializeValue)
        {
            if (initialized)
            {
                // TODO: dispose managed state (managed objects).
            }

            string path = Application.persistentDataPath + "/Logs/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            attackerWriter = new StreamWriter(path + "Attacker" + System.DateTime.Now + ".txt");
            attackerWriter.WriteLine("Event:");

            defenderWriter = new StreamWriter(path + "Defender" + System.DateTime.Now + ".txt");
            defenderWriter.WriteLine("Event:");

            disposedValue = false;
            initializeValue = true;
        }
    }    
}
