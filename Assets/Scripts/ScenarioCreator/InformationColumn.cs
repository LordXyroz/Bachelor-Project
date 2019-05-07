using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for displaying information about a node in the scenario creator.
/// </summary>
public class InformationColumn : MonoBehaviour
{

    [Header("Text items")]
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text securityLevelText;
    [SerializeField]
    private Text entryPointText;

    [Header("Objects")]
    [SerializeField]
    private GameObject infoAreaObject;
    [SerializeField]
    private GameObject vulnerabilityPrefab;

    [Header("Lists")]
    private List<GameObject> vulnerabilityObjects = new List<GameObject>();

    /// <summary>
    /// Called on start, makes sure information is cleared.
    /// </summary>
    void Start()
    {
        ClearInformationColumn();
    }

    /// <summary>
    /// Populates the column with info about a node.
    /// </summary>
    /// <param name="name">Name of the node</param>
    /// <param name="vulnerabilities">Vulnerabilities added to the node</param>
    /// <param name="security">Security level of the node</param>
    /// <param name="entryPoint">If the node is an entry point</param>
    public void PopulateInformationColumn(string name, List<AttackTypes> vulnerabilities, int security, bool entryPoint)
    {
        nameText.text = name;
        securityLevelText.text = "Level: " + security.ToString();
        entryPointText.text = (entryPoint ? "Yes" : "No");
        
        foreach (var v in vulnerabilities)
        {
            var go = Instantiate(vulnerabilityPrefab, infoAreaObject.transform);
            go.GetComponentInChildren<Text>().text = VulnerabilityLogic.GetAttackString(v);

            vulnerabilityObjects.Add(go);
        }

        infoAreaObject.SetActive(true);
    }

    /// <summary>
    /// Overload of <see cref="PopulateInformationColumn(string, List{AttackTypes}, int, bool)"/>
    /// </summary>
    /// <param name="comp">The node to display info of</param>
    public void PopulateInformationColumn(SystemComponent comp)
    {
        nameText.text = comp.componentName;
        securityLevelText.text = "Level: " + comp.securityLevel.ToString();
        entryPointText.text = (comp.isEntryPoint ? "Yes" : "No");

        foreach (var v in comp.componentVulnerabilities)
        {
            var go = Instantiate(vulnerabilityPrefab, infoAreaObject.transform);
            go.GetComponentInChildren<Text>().text = VulnerabilityLogic.GetAttackString(v);

            vulnerabilityObjects.Add(go);
        }

        infoAreaObject.SetActive(true);
    }

    /// <summary>
    /// Disables the column, and destroys previously added vulnerabilityObjects.
    /// </summary>
    public void ClearInformationColumn()
    {
        infoAreaObject.SetActive(false);

        for (int i = vulnerabilityObjects.Count - 1; i >= 0; i--)
            Destroy(vulnerabilityObjects[i]);

        vulnerabilityObjects.Clear();
    }
}
