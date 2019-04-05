using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Script for the functionality behind saving relevant scenario data to JSON
/// </summary>
public class SaveScenario : MonoBehaviour
{
    private List<GameObject> systemComponentsToSave = new List<GameObject>();
    private List<GameObject> connectedComponents = new List<GameObject>();
    private List<GameObject> referenceLines = new List<GameObject>();
    private DropZone dropZone;
    private Canvas canvas;
    private SelectedObject selectedObject;
    private ConnectionReferences lineReference;

    [Header("Save file attributes")]
    public bool overrideFile = false;
    public string fileName;
    private string directoryPath;
    private string filePath;
    private bool entryPointExists;
    private SaveMenu saveMenu;
    private SavefileExistsPopup fileExistsPopup;
    private SaveError saveErrorMessage;

    [SerializeField]
    private Save save;

    [Header("The stats for attacker and defender")]
    private int attackerAttackLevel;
    private int attackerDiscoveryLevel;
    private int attackerAnalysisLevel;
    private int defenderDefenceLevel;
    private int defenderDiscoveryLevel;

    private int attackerResources;
    private int defenderResources;

    private void Start()
    {
        entryPointExists = false;
        directoryPath = Application.dataPath + "/Savefiles";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        canvas = GetComponentInParent<Canvas>();
        fileExistsPopup = canvas.transform.GetComponentInChildren<SavefileExistsPopup>(true);
    }

    private Save CreateSaveScenarioObject()
    {
        dropZone = FindObjectOfType<DropZone>();
        systemComponentsToSave = dropZone.editableSystemComponents;


        if (!systemComponentsToSave.Count.Equals(0))
        {
            entryPointExists = false;
            foreach (GameObject component in systemComponentsToSave)
            {
                if (component.GetComponent<SystemComponent>().isEntryPoint)
                    entryPointExists = true;
            }
            if (entryPointExists)
            {
                save = new Save();
                saveMenu = canvas.transform.GetComponentInChildren<SaveMenu>(true);
                selectedObject = FindObjectOfType<SelectedObject>();

                if (saveMenu.filename != null)
                {
                    fileName = saveMenu.filename;
                }
                filePath = Path.Combine(directoryPath, saveMenu.filename);
                if (!filePath.EndsWith(".json"))
                {
                    filePath += ".json";
                }

                /// Attacker stats
                if (saveMenu.attackerAttackLevel >= 1 && saveMenu.attackerAttackLevel <= 3)
                    save.attackerAttackLevel = saveMenu.attackerAttackLevel;
                else
                    save.attackerAttackLevel = 1;

                if (saveMenu.attackerAnalysisLevel >= 1 && saveMenu.attackerAnalysisLevel <= 3)
                    save.attackerAnalysisLevel = saveMenu.attackerAnalysisLevel;
                else
                    save.attackerAnalysisLevel = 1;

                if (saveMenu.attackerDiscoveryLevel >= 1 && saveMenu.attackerDiscoveryLevel <= 3)
                    save.attackerDiscoveryLevel = saveMenu.attackerDiscoveryLevel;
                else
                    save.attackerDiscoveryLevel = 1;

                if (saveMenu.attackerResources >= systemComponentsToSave.Count * 30 && saveMenu.attackerResources <= systemComponentsToSave.Count * 100)
                    save.attackerResources = saveMenu.attackerResources;
                else
                    save.attackerResources = systemComponentsToSave.Count * 100;

                /// Defender stats
                if (saveMenu.defenderDefenseLevel >= 1 && saveMenu.defenderDefenseLevel <= 3)
                    save.defenderDefenceLevel = saveMenu.defenderDefenseLevel;
                else
                    save.defenderDefenceLevel = 1;

                if (saveMenu.defenderDiscoveryLevel >= 1 && saveMenu.defenderDiscoveryLevel <= 3)
                    save.defenderAnalysisLevel = saveMenu.defenderDiscoveryLevel;
                else
                    save.defenderAnalysisLevel = 1;

                if (saveMenu.defenderResources >= systemComponentsToSave.Count * 20 && saveMenu.defenderResources <= systemComponentsToSave.Count * 80)
                    save.defenderResources = saveMenu.defenderResources;
                else
                    save.defenderResources = systemComponentsToSave.Count * 80;

                /// Game stats
                if (saveMenu.gameTimeInMinuttes >= 0)
                    save.gameTimeInMinuttes = saveMenu.gameTimeInMinuttes;
                else
                    save.gameTimeInMinuttes = 30;   // Default 30min

                foreach (GameObject targetGameObject in systemComponentsToSave)
                {
                    VulnerabilityWrapper vulnerabilityWrapper = new VulnerabilityWrapper();

                    GameObject target = targetGameObject.gameObject;
                    SystemComponent targetComponent = target.GetComponent<SystemComponent>();
                    if (target != null)
                    {
                        save.systemComponentNamesList.Add(targetComponent.componentName);
                        save.systemComponentPositionsList.Add(target.transform.position);
                        save.systemComponentTypesList.Add(targetComponent.componentType);
                        save.systemComponentSecurityLevelsList.Add(targetComponent.securityLevel);
                        save.isEntryPointList.Add(targetComponent.isEntryPoint);

                        foreach (var vulnerability in targetComponent.componentVulnerabilities)
                        {
                            vulnerabilityWrapper.vulnerabilityWrapperList.Add(vulnerability);
                        }
                        save.systemComponentVulnerabilyWrappersList.Add(vulnerabilityWrapper);

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
                referenceLines = selectedObject.connectionReferencesList;
                foreach (GameObject line in referenceLines)
                {
                    save.connectionLinePosition.Add(line.transform.position);
                    lineReference = line.gameObject.GetComponent<ConnectionReferences>();

                    save.referenceFromObject.Add(lineReference.referenceFromObject);
                    save.referenceToObject.Add(lineReference.referenceToObject);
                    save.referenceFromObjectName.Add(lineReference.referenceFromObject.name);
                    save.referenceToObjectName.Add(lineReference.referenceToObject.name);
                    save.hasFirewall.Add(lineReference.hasFirewall);
                    save.firewallPositionsList.Add(lineReference.transform.position);
                }
                return save;
            }
            else
            {
                saveErrorMessage = canvas.transform.GetComponentInChildren<SaveError>(true);
                saveErrorMessage.gameObject.SetActive(true);
                saveErrorMessage.ChangeErrorMessage("Missing entry point. There must be at least 1 entry point. ");
                return null;
            }
        }
        else
        {
            saveErrorMessage = canvas.transform.GetComponentInChildren<SaveError>(true);
            saveErrorMessage.gameObject.SetActive(true);
            saveErrorMessage.ChangeErrorMessage("Nothing to save ");
            return null;
        }
    }

    /// <summary>
    /// Save the current scenario to a JSON file
    /// </summary>
    public void SaveCurrentScenario()
    {
        save = CreateSaveScenarioObject();

        if (save != null)
        {
            string json = JsonUtility.ToJson(save);
            Debug.Log("Saving as JSON: " + json);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
                File.WriteAllText(filePath, json);
            }
            else
            {
                if (overrideFile)
                {
                    overrideFile = false;
                    File.Delete(filePath);
                    File.Create(filePath).Dispose();
                    File.WriteAllText(filePath, json);

                }
                else
                {
                    fileExistsPopup.gameObject.SetActive(true);
                }
            }
        }
    }
}
