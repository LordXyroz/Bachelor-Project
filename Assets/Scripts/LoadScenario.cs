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
    private SaveMenu saveMenu;
    private Canvas canvas;
    [SerializeField]
    Save loadFromJSON;

    public string directoryPath;
    public string fileName;
    private string filePath;
    private string resourcePath;
    private string prefabType;
    private int prefabNo = 0;

    [Header("The instantiated objects")]
    GameObject target;
    GameObject line;
    Vector2 pos = new Vector2(300, 650);


    [Header("Prefabs for instantiation")]
    public GameObject ChosenPrefab;
    public GameObject DefaultPrefab;
    public GameObject APIPrefab;
    public GameObject connectionLinePrefab;
    public GameObject InternetPrefab;
    public GameObject SwitchPrefab;
    public GameObject WebsitePrefab;



    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        saveMenu = canvas.transform.GetComponentInChildren<SaveMenu>(true);
        selectedObject = FindObjectOfType<SelectedObject>();
        dropZone = FindObjectOfType<DropZone>();

        directoryPath = Application.dataPath + "/Savefiles";
        filePath = Path.Combine(directoryPath, saveMenu.filename + ".json");
    }


    public void LoadGame(string chosenFilepath)
    {
        filePath = chosenFilepath;

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
            ///////////////////////////////////////////////////////////////////////////////////////////

            string json = File.ReadAllText(filePath);
            loadFromJSON = JsonUtility.FromJson<Save>(json);

            saveMenu.filename = fileName;
            saveMenu.attackerAttackLevel = loadFromJSON.attackerAttackLevel;
            saveMenu.attackerAnalysisLevel = loadFromJSON.attackerAnalysisLevel;
            saveMenu.attackerDiscoveryLevel = loadFromJSON.attackerDiscoveryLevel;
            saveMenu.attackerResources = loadFromJSON.attackerResources;

            saveMenu.defenderDefenseLevel = loadFromJSON.defenderDefenceLevel;
            saveMenu.defenderDiscoveryLevel = loadFromJSON.defenderDiscoveryLevel;
            saveMenu.defenderResources = loadFromJSON.defenderResources;

            for (int i = 0; i < loadFromJSON.systemComponentPositionsList.Count; i++)
            {
                VulnerabilityWrapper vulnerabilityWrapper = loadFromJSON.systemComponentVulnerabilyWrappersList[i];

                pos = loadFromJSON.systemComponentPositionsList[i];
                LoadObject(loadFromJSON.systemComponentTypesList[i]);
                SystemComponent targetComponent = target.GetComponent<SystemComponent>();
                targetComponent.componentType = loadFromJSON.systemComponentTypesList[i];

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
    }


    public void LoadObject(string componentType)
    {
        switch (componentType)
        {
            case "API":
                InstantiateObject(APIPrefab);
                break;

            case "Internet":
                InstantiateObject(InternetPrefab);
                break;

            case "Switch":
                InstantiateObject(SwitchPrefab);
                break;

            case "Website":
                InstantiateObject(WebsitePrefab);
                break;

            case "System component":
                InstantiateObject(DefaultPrefab);
                break;

            default:
                Debug.Log("Failed recognizing component type: " + componentType +
                    ". This is not in the list of recognized component types (LoadScenario.LoadObject(string componentType)");
                InstantiateObject(DefaultPrefab);
                break;
        }
    }


    public void InstantiateObject(GameObject prefab)
    {
        target = (GameObject)Instantiate(prefab,
                                         pos,
                                         Quaternion.identity,
                                         dropZone.transform);
        target.name = target.name + prefabNo++;
        dropZone.editableSystemComponents.Add(target);
    }


    /// <summary>
    /// Need separate function for each prefab type in order to link the functions to separate buttons
    /// </summary>
    public void InstantiateAPIButton()
    {
        pos = new Vector2(300, 650);
        InstantiateObject(APIPrefab);
    }


    /// <summary>
    /// Need separate function for each prefab type in order to link the functions to separate buttons
    /// </summary>
    public void InstantiateInternetButton()
    {
        pos = new Vector2(300, 650);
        InstantiateObject(InternetPrefab);
    }


    /// <summary>
    /// Need separate function for each prefab type in order to link the functions to separate buttons
    /// </summary>
    public void InstantiateSwitchButton()
    {
        pos = new Vector2(300, 650);
        InstantiateObject(SwitchPrefab);
    }


    /// <summary>
    /// Need separate function for each prefab type in order to link the functions to separate buttons
    /// </summary>
    public void InstantiateWebsiteButton()
    {
        pos = new Vector2(300, 650);
        InstantiateObject(WebsitePrefab);
    }
}
