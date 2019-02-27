using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationColumn : MonoBehaviour
{

    private Canvas canvas;
    private TMP_Text currentObjectName;
    private TMP_Text securityLevel;
    private TMP_Text vulnerabilityHeader;
    private TMP_Text currentVulnerabilities;


    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        currentObjectName = canvas.transform.Find("InformationColumnRight").transform.Find("ComponentNameText").GetComponent<TMP_Text>();
        securityLevel = canvas.transform.Find("InformationColumnRight").transform.Find("SecurityLevelText").GetComponent<TMP_Text>();
        vulnerabilityHeader = canvas.transform.Find("InformationColumnRight").transform.Find("VulnerabilityListText").GetComponent<TMP_Text>();
        currentVulnerabilities = canvas.transform.Find("InformationColumnRight").transform.Find("VulnerabilitiesText").GetComponent<TMP_Text>();
        ClearInformationColumn();
    }


    public void PopulateInformationColumn(string name, List<string> vulnerabilities, int security)
    {
        currentObjectName.text = name;
        vulnerabilityHeader.text = "Vulnerabilities";
        securityLevel.text = "Security level: " + security.ToString();

        if (vulnerabilities.Count == 0)
        {
            currentVulnerabilities.text = "None";
        }
        else
        {
            foreach (string vulnerability in vulnerabilities)
            {
                currentVulnerabilities.text += vulnerability + "\n";
            }
        }
    }


    public void ClearInformationColumn()
    {
        currentObjectName.text = "";
        securityLevel.text = "";
        vulnerabilityHeader.text = "";
        currentVulnerabilities.text = "";
    }
}
