using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadScenario : MonoBehaviour
{
    private List<GameObject> existingSystemComponents = new List<GameObject>();
    private DropZone dropZone;
    private SelectedObject selectedObject;
    private string fileName = "savefile";
    private string filePath;
    private string resourcePath;
    private string prefabType;

    public GameObject APIPrefab;
    private int prefabNo = 0;


    private void Start()
    {
        selectedObject = FindObjectOfType<SelectedObject>();
        dropZone = FindObjectOfType<DropZone>();
    }


    public void LoadGame()
    {
        filePath = Path.Combine(Application.dataPath + "/Savefiles", fileName + ".json");
        Debug.Log("Filepath from load: " + filePath);

        if (File.Exists(filePath))
        {
            existingSystemComponents = dropZone.editableSystemComponents;

            /// Delete all existing components upon loading, before populating from the saved file
            for (int i = existingSystemComponents.Count - 1; i >= 0; i--)
            {
                existingSystemComponents[i].GetComponent<SystemComponent>().DeleteSystemComponent();
            }

            string json = File.ReadAllText(filePath);
            Save loadFromJSON = JsonUtility.FromJson<Save>(json);

            for (int i = 0; i < loadFromJSON.systemComponentPositionsList.Count; i++)
            {
                VulnerabilityWrapper vulnerabilityWrapper = loadFromJSON.systemComponentVulnerabilyWrappersList[i];
                ConnectedComponentsWrapper connectedComponentWrapper = loadFromJSON.systemComponentConnectedComponentsWrapperList[i];

                GameObject target = (GameObject)Instantiate(/*Resources.Load*/(APIPrefab),
                                                                    loadFromJSON.systemComponentPositionsList[i],
                                                                    Quaternion.identity,
                                                                    dropZone.transform);
                SystemComponent targetComponent = target.GetComponent<SystemComponent>();

                targetComponent.securityLevel = loadFromJSON.systemComponentSecurityLevelsList[i];
                targetComponent.componentName = loadFromJSON.systemComponentNamesList[i];

                foreach (string vulnerability in vulnerabilityWrapper.vulnerabilityWrapperList)
                {
                    targetComponent.componentVulnerabilities.Add(vulnerability);
                }
                //foreach (string vulnerability in targetComponent.componentVulnerabilities)
                //{
                //    vulnerabilityWrapper.vulnerabilityWrapperList.Add(vulnerability);
                //}

                foreach (GameObject connectedObject in connectedComponentWrapper.connectedObjectWrapperList)
                {
                    //GameObject connected;
                    //connected = GameObject.Find(connectedObject);
                    //Debug.Log("Connected to gameobject: " + target.name + "  -  is: " + connected.name);
                    selectedObject.connectionStarted = true;
                    //selectedObject.ConnectObjects(target, connectedObject);
                }
                //foreach (GameObject connectedObject in targetComponent.GetConnectedComponents())
                //{
                //    connectedComponentWrapper.connectedObjectWrapperList.Add(connectedObject.name);
                //}
                //save.systemComponentConnectedComponents.Add(connectedComponentWrapper);
                //

                /// Empty objects intended for later development cycles
                targetComponent.OS = loadFromJSON.OSList[i];
                targetComponent.subnet = loadFromJSON.subnetList[i];
                targetComponent.configPresent = loadFromJSON.configPresentList[i];
                targetComponent.keypair = loadFromJSON.keypairList[i];
                targetComponent.floatingIP = loadFromJSON.floatingIPList[i];
                targetComponent.user = loadFromJSON.usersList[i];
                /////////////////////////////////////////////////////

                dropZone.editableSystemComponents.Add(target);
                //Debug.Log("Loaded component: " + target.name);
            }

            Debug.Log("Game Loaded: " + json);
        }
        else
        {
            Debug.Log("File missing: " + filePath);
        }
    }

    public void InstantiateObject()
    {

        GameObject systemComponentClone = (GameObject)Instantiate(APIPrefab,
                                                                    new Vector2(300, 650),
                                                                    Quaternion.identity,
                                                                    dropZone.transform);
        systemComponentClone.name = systemComponentClone.name + prefabNo++;

        dropZone.editableSystemComponents.Add(systemComponentClone);


        //Image systemComponentClone = (Image)Instantiate(systemComponentImage, parentToReturnTo);
        //systemComponentClone.transform.position = (new Vector2(50, 600));
        //systemComponentClone.Imagecolor = originalColor;
    }
}
