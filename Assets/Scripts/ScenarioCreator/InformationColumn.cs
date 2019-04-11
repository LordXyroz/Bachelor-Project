using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


    void Start()
    {
        ClearInformationColumn();
    }


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


    public void ClearInformationColumn()
    {
        infoAreaObject.SetActive(false);

        for (int i = vulnerabilityObjects.Count - 1; i >= 0; i--)
            Destroy(vulnerabilityObjects[i]);

        vulnerabilityObjects.Clear();
    }
}
