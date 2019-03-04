using System.Collections.Generic;
using UnityEngine;

public class SaveScenario : MonoBehaviour
{
    private List<GameObject> systemComponentsToSave = new List<GameObject>();
    private DropZone dropZone;


    private Save CreateSaveScenarioObject()
    {
        dropZone = FindObjectOfType<DropZone>();

        systemComponentsToSave = dropZone.editableSystemComponents;
        Save save = new Save();
        List<string> vulnerabilities = new List<string>();

        foreach (GameObject targetGameObject in systemComponentsToSave)
        {
            VulnerabilityWrapper vulnerabilityWrapper = new VulnerabilityWrapper();
            ConnectedComponentsWrapper connectedComponentWrapper = new ConnectedComponentsWrapper();

            GameObject target = targetGameObject.gameObject;
            SystemComponent targetComponent = target.GetComponent<SystemComponent>();
            if (target != null)
            {
                save.systemComponentPositionsList.Add(target.transform.position);
                save.systemComponentTypesList.Add(targetComponent.componentType);
                save.systemComponentSecurityLevelsList.Add(targetComponent.securityLevel);

                foreach (string vulnerability in targetComponent.componentVulnerabilities)
                {
                    vulnerabilityWrapper.vulnerabilityWrapperList.Add(vulnerability);
                }
                save.systemComponentVulnerabilitiesList.Add(vulnerabilityWrapper);

                foreach (GameObject connectedObject in targetComponent.GetConnectedComponents())
                {
                    connectedComponentWrapper.connectedObjectWrapperList.Add(connectedObject.name);
                }
                save.systemComponentConnectedComponents.Add(connectedComponentWrapper);

                /// Empty objects intended for later development cycles
                save.OSList.Add(targetComponent.OS);
                save.subnetList.Add(targetComponent.subnet);
                save.configPresentList.Add(targetComponent.configPresent);
                save.keypairList.Add(targetComponent.keypair);
                save.floatingIPList.Add(targetComponent.floatingIP);
                save.usersList.Add(targetComponent.user);
            }
        }

        return save;
    }

    public void SaveCurrentScenario()
    {
        Save save = CreateSaveScenarioObject();

        //FileStream file = File.Create(Application.persistentDataPath + "/scenariosave.save");
        /*BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, save);

        Debug.Log("Scenario saved");*/
        string json = JsonUtility.ToJson(save);

        ///convert the JSON back into an instance of Save
        Save saveFromJSON = JsonUtility.FromJson<Save>(json);

        Debug.Log("Saving as JSON: " + json);
        //file.Close();
    }
}
