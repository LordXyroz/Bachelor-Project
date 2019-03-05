using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveScenario : MonoBehaviour
{
    private List<GameObject> systemComponentsToSave = new List<GameObject>();
    private List<GameObject> connectedComponents = new List<GameObject>();
    private DropZone dropZone;
    private string fileName = "savefile";
    private string filePath;
    private Save save;


    //private void Awake()
    //{
    //    if (save == null)
    //    {
    //        save = new Save();
    //    }
    //
    //    filePath = Path.Combine(Application.dataPath + "/Savefiles", fileName + ".json");
    //    Debug.Log("Filepath from save: " + filePath);
    //}


    private Save CreateSaveScenarioObject()
    {
        dropZone = FindObjectOfType<DropZone>();
        systemComponentsToSave = dropZone.editableSystemComponents;

        if (!systemComponentsToSave.Count.Equals(0))
        {
            filePath = Path.Combine(Application.dataPath + "/Savefiles", fileName + ".json");
            Debug.Log("Filepath from save: " + filePath);

            save = new Save();
            foreach (GameObject targetGameObject in systemComponentsToSave)
            {
                //List<string> vulnerabilities = new List<string>();

                VulnerabilityWrapper vulnerabilityWrapper = new VulnerabilityWrapper();
                ConnectedComponentsWrapper connectedComponentWrapper = new ConnectedComponentsWrapper();

                GameObject target = targetGameObject.gameObject;
                SystemComponent targetComponent = target.GetComponent<SystemComponent>();
                if (target != null)
                {
                    save.systemComponentNamesList.Add(targetComponent.componentName);
                    save.systemComponentPositionsList.Add(target.transform.position);
                    save.systemComponentTypesList.Add(targetComponent.componentType);
                    save.systemComponentSecurityLevelsList.Add(targetComponent.securityLevel);

                    foreach (string vulnerability in targetComponent.componentVulnerabilities)
                    {
                        vulnerabilityWrapper.vulnerabilityWrapperList.Add(vulnerability);
                    }
                    save.systemComponentVulnerabilyWrappersList.Add(vulnerabilityWrapper);

                    connectedComponents = targetComponent.GetConnectedComponents();
                    foreach (GameObject connectedObject in connectedComponents)
                    {
                        /*string name = connectedObject.name;
                        connectedComponentWrapper.connectedObjectWrapperList.Add(name);*/
                        connectedComponentWrapper.connectedObjectWrapperList.Add(connectedObject);
                    }
                    save.systemComponentConnectedComponentsWrapperList.Add(connectedComponentWrapper);

                    /// Empty objects intended for later development cycles
                    save.OSList.Add(targetComponent.OS);
                    save.subnetList.Add(targetComponent.subnet);
                    save.configPresentList.Add(targetComponent.configPresent);
                    save.keypairList.Add(targetComponent.keypair);
                    save.floatingIPList.Add(targetComponent.floatingIP);
                    save.usersList.Add(targetComponent.user);
                    ///////////////////////////////////////////////////////
                }

            }
            return save;
        }
        else
        {
            Debug.LogWarning("Nothing to save");
            return null;
        }
    }

    public void SaveCurrentScenario()
    {
        Save save = CreateSaveScenarioObject();
        //if (!File.Exists(filePath))
        //{
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log("Deleted file: " + filePath);
        }
        if (save != null)
        {
            File.Create(filePath).Dispose();
            string json = JsonUtility.ToJson(save);

            //Debug.Log("Saving as JSON: " + json);
            File.WriteAllText(filePath, json);
        }
        //}
        //else
        //{
        //    Debug.Log("There already exist a file: " + filePath);
        //}
    }
}
