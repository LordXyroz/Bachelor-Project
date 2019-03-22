using UnityEngine;

[System.Serializable]
public class SaveFileMessage : Message
{
    public string fileString;

    public SaveFileMessage(string target, string sender, ushort type, Save save)
        : base(target, sender, type)
    {
        fileString = JsonUtility.ToJson(save);
    }
}
