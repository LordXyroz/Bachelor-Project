using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadScenario : MonoBehaviour
{
    private List<GameObject> systemComponentsToSave = new List<GameObject>();
    private DropZone dropZone;

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {

            dropZone = FindObjectOfType<DropZone>();
            systemComponentsToSave = dropZone.editableSystemComponents;

            /// change to load JSON file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < save.systemComponentPositions.Count; i++)
            {
                Vector3 position = save.systemComponentPositions[i];
                GameObject target = systemComponentsToSave[i].GetComponent<GameObject>();
                //target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
            }

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}
