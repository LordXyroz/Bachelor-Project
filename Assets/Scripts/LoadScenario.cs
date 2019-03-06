using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Script for loading an existing game file.
/// Unknown error causes in-game load to loose reference line references upon second load. Make 1 load available at a time for the moment
/// </summary>
public class LoadScenario : MonoBehaviour
{
    [Header("List of active gameobjects, in order to delete old objects when loading a file")]
    private List<GameObject> existingSystemComponents = new List<GameObject>();
    private List<GameObject> existingConnectionLines = new List<GameObject>();

    [Header("References to other scripts")]
    private DropZone dropZone;
    private SelectedObject selectedObject;
    private ConnectionReferences lineReference;
    [SerializeField]
    Save loadFromJSON;

    private string fileName = "savefile";
    private string filePath;
    private string resourcePath;
    private string prefabType;
    private int prefabNo = 0;

    [Header("The instantiated objects")]
    GameObject target;
    GameObject line;
    Vector2 pos = new Vector2(300, 650);


    [Header("Prefabs for instantiation")]
    public GameObject APIPrefab;
    public GameObject connectionLinePrefab;



    private void Start()
    {
        selectedObject = FindObjectOfType<SelectedObject>();
        dropZone = FindObjectOfType<DropZone>();
    }


    public void LoadGame()
    {
        filePath = Path.Combine(Application.dataPath + "/Savefiles", fileName + ".json");
        //Debug.Log("Filepath from load: " + filePath);

        if (File.Exists(filePath))
        {
            existingConnectionLines = selectedObject.connectionReferencesList;
            existingSystemComponents = dropZone.editableSystemComponents;

            /// Delete all existing components upon loading, before populating from the saved file
            for (int i = existingConnectionLines.Count - 1; i >= 0; i--)
            {
                existingConnectionLines[i].GetComponent<ConnectionReferences>().RemoveConnectionComponent();
            }
            for (int i = existingSystemComponents.Count - 1; i >= 0; i--)
            {
                existingSystemComponents[i].GetComponent<SystemComponent>().DeleteSystemComponent();
            }
            prefabNo = 0;
            Debug.Log("Components in delete should be 0, are: " + dropZone.editableSystemComponents.Count);
            Debug.Log("Connections in delete should be 0, are: " + selectedObject.connectionReferencesList.Count);

            string json = File.ReadAllText(filePath);
            loadFromJSON = JsonUtility.FromJson<Save>(json);

            for (int i = 0; i < loadFromJSON.systemComponentPositionsList.Count; i++)
            {
                VulnerabilityWrapper vulnerabilityWrapper = loadFromJSON.systemComponentVulnerabilyWrappersList[i];

                pos = loadFromJSON.systemComponentPositionsList[i];
                InstantiateObject();
                SystemComponent targetComponent = target.GetComponent<SystemComponent>();

                targetComponent.securityLevel = loadFromJSON.systemComponentSecurityLevelsList[i];
                targetComponent.componentName = loadFromJSON.systemComponentNamesList[i];

                foreach (string vulnerability in vulnerabilityWrapper.vulnerabilityWrapperList)
                {
                    targetComponent.componentVulnerabilities.Add(vulnerability);
                }

                /// Empty objects intended for later development cycles
                targetComponent.OS = loadFromJSON.OSList[i];
                targetComponent.subnet = loadFromJSON.subnetList[i];
                targetComponent.configPresent = loadFromJSON.configPresentList[i];
                targetComponent.keypair = loadFromJSON.keypairList[i];
                targetComponent.floatingIP = loadFromJSON.floatingIPList[i];
                targetComponent.user = loadFromJSON.usersList[i];
                /////////////////////////////////////////////////////
            }

            /// Must be run after the object instantiations, in order to connect them correctly
            for (int i = 0; i < loadFromJSON.hasFirewall.Count; i++)
            {
                line = (GameObject)Instantiate(connectionLinePrefab,
                                         loadFromJSON.connectionLinePosition[i],
                                         Quaternion.identity,
                                         dropZone.transform.Find("Connections").gameObject.transform);

                selectedObject.connectionReferencesList.Add(line);
                lineReference = line.gameObject.GetComponent<ConnectionReferences>();

                //lineReference.referenceFromObject = loadFromJSON.referenceFromObject[i];
                //lineReference.referenceToObject = loadFromJSON.referenceToObject[i];
                lineReference.referenceFromObject = GameObject.Find(loadFromJSON.referenceFromObjectName[i]);
                lineReference.referenceToObject = GameObject.Find(loadFromJSON.referenceToObjectName[i]);

                lineReference.referenceFromObject.GetComponent<SystemComponent>().connectedReferenceLines.Add(line);
                lineReference.referenceToObject.GetComponent<SystemComponent>().connectedReferenceLines.Add(line);

                lineReference.hasFirewall = loadFromJSON.hasFirewall[i];
                lineReference.referenceFromObject.GetComponent<SystemComponent>().SetConnectionLines(lineReference.referenceFromObject.gameObject.transform.position,
                                                                                                     lineReference.referenceToObject.gameObject.transform.position,
                                                                                                     line);
            }
            Debug.Log("Game Loaded: " + json);
        }
        else
        {
            Debug.Log("File missing: " + filePath);
        }
        //foreach (GameObject line in selectedObject.connectionReferencesList)  // TODO delete, for debugging only
        //{
        //    Debug.Log("References for object: " + line.name + " - is: " +
        //        line.GetComponent<ConnectionReferences>().referenceFromObject + " and " +
        //        line.GetComponent<ConnectionReferences>().referenceToObject);
        //}
    }

    public void InstantiateObject()
    {

        target = (GameObject)Instantiate(APIPrefab,
                                         pos,
                                         Quaternion.identity,
                                         dropZone.transform);
        target.name = target.name + prefabNo++;

        dropZone.editableSystemComponents.Add(target);


        //Image systemComponentClone = (Image)Instantiate(systemComponentImage, parentToReturnTo);
        //systemComponentClone.transform.position = (new Vector2(50, 600));
        //systemComponentClone.Imagecolor = originalColor;
    }

    public void InstantiateButton()
    {
        pos = new Vector2(300, 650);
    }
}
