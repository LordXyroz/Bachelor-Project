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
    private SaveMenu saveMenu;
    private SavefileExistsPopup fileExistsPopup;

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
        directoryPath = Application.dataPath + "/Savefiles";

        canvas = GetComponentInParent<Canvas>();
        fileExistsPopup = canvas.transform.GetComponentInChildren<SavefileExistsPopup>(true);
    }

    private Save CreateSaveScenarioObject()
    {
        dropZone = FindObjectOfType<DropZone>();
        systemComponentsToSave = dropZone.editableSystemComponents;


        if (!systemComponentsToSave.Count.Equals(0))
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
            save.attackerAttackLevel = saveMenu.attackerAttackLevel;
            save.attackerAnalysisLevel = saveMenu.attackerAnalysisLevel;
            save.attackerDiscoveryLevel = saveMenu.attackerDiscoveryLevel;
            save.attackerResources = saveMenu.attackerResources;

            save.defenderDefenceLevel = saveMenu.defenderDefenseLevel;
            save.defenderDiscoveryLevel = saveMenu.defenderDiscoveryLevel;
            save.defenderResources = saveMenu.defenderResources;

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

                    foreach (string vulnerability in targetComponent.componentVulnerabilities)
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
            Debug.LogWarning("Nothing to save");
            return null;
        }
    }


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
                //Debug.Log("There already exist a file: " + filePath);
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
