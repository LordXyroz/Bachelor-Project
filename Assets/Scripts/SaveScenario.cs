using System.Collections.Generic;
using UnityEngine;

public class SaveScenario : MonoBehaviour
{
    private List<GameObject> systemComponentsToSave = new List<GameObject>();
    private DropZone dropZone;

    private void Start()
    {
    }
    private Save CreateSaveScenarioObject()
    {
        dropZone = FindObjectOfType<DropZone>();

        systemComponentsToSave = dropZone.editableSystemComponents;
        Save save = new Save();
        foreach (GameObject targetGameObject in systemComponentsToSave)
        {
            GameObject target = targetGameObject.gameObject;
            Debug.Log("Target for save is: " + target.name);
            if (target != null)
            {
                save.systemComponentPositions.Add(target.transform.position);
                //save.systemComponentTypes.Add((int)target.GetComponent<SelectedObject>().type);
                //Debug.Log("Saving data: " + target.transform.position.x + " - " + target.transform.position.y + " - " + target.transform.position.z);
            }
        }
        Debug.Log("Saving data: " + save.systemComponentPositions[0]);

        return save;
    }

    public void SaveCurrentScenario()
    {
        Save save = CreateSaveScenarioObject();

        /*BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/scenariosave.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Scenario saved");*/
        string json = JsonUtility.ToJson(save);

        ///convert the JSON back into an instance of Save
        Save saveFromJSON = JsonUtility.FromJson<Save>(json);

        Debug.Log("Saving as JSON: " + json);
    }
}
