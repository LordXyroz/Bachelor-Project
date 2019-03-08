using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveScenario : MonoBehaviour
{
    private List<GameObject> systemComponentsToSave = new List<GameObject>();
    private List<GameObject> connectedComponents = new List<GameObject>();
    private List<GameObject> referenceLines = new List<GameObject>();
    private DropZone dropZone;
    private SelectedObject selectedObject;
    private ConnectionReferences lineReference;
    private SaveMenu saveMenu;
    private string fileName = "savefile";
    private string filePath;

    [SerializeField]
    private Save save;

    [Header("The stats for attacker and defender")]
    private int attackerAttackLevel;
    private int attackerDiscoveryLevel;
    private int attackerAnalysisLevel;
    private int defenderDefenceLevel;
    private int defenderDiscoveryLevel;



    private Save CreateSaveScenarioObject()
    {
        dropZone = FindObjectOfType<DropZone>();
        systemComponentsToSave = dropZone.editableSystemComponents;
        saveMenu = FindObjectOfType<SaveMenu>();


        if (!systemComponentsToSave.Count.Equals(0))
        {
            save = new Save();
            if (saveMenu.filename != null)
            {
                fileName = saveMenu.filename;
            }
            selectedObject = FindObjectOfType<SelectedObject>();
            filePath = Path.Combine(Application.dataPath + "/Savefiles", fileName + ".json");
            Debug.Log("Filepath from save: " + filePath);

            save.attackerAttackLevel = saveMenu.attackerAttackLevel;
            save.attackerAnalysisLevel = saveMenu.attackerAnalysisLevel;
            save.attackerDiscoveryLevel = saveMenu.attackerDiscoveryLevel;
            save.defenderDefenceLevel = saveMenu.defenderDefenseLevel;
            save.defenderDiscoveryLevel = saveMenu.defenderDiscoveryLevel;

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

            Debug.Log("Saving as JSON: " + json);
            File.WriteAllText(filePath, json);
        }
        //}
        //else
        //{
        //    Debug.Log("There already exist a file: " + filePath);
        //}
    }
}
